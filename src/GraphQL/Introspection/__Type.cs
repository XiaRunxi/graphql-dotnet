using System;
using System.Linq;
using GraphQL.Types;

namespace GraphQL.Introspection
{
    public class __Type : ObjectGraphType
    {
        public __Type()
        {
            Name = "__Type";
            Field<NonNullGraphType<__TypeKind>>("kind", null, null, context =>
            {
                if (context.Source is GraphType)
                {
                    return KindForInstance((GraphType)context.Source);
                }
                else if (context.Source is Type)
                {
                    return KindForType((Type)context.Source);
                }

                throw new ExecutionError("Unkown kind of type: {0}".ToFormat(context.Source));
            });
            Field<StringGraphType>("name", resolve: context =>
            {
                if (context.Source is Type)
                {
                    var type = (Type) context.Source;

                    if (typeof (NonNullGraphType).IsAssignableFrom(type)
                        || typeof (ListGraphType).IsAssignableFrom(type))
                    {
                        return null;
                    }

                    var resolved = context.Schema.FindType(type);
                    return resolved.Name;
                }

                return ((GraphType) context.Source).Name;
            });
            Field<StringGraphType>("description");
            Field<ListGraphType<NonNullGraphType<__Field>>>("fields", null,
                new QueryArguments(new[]
                {
                    new QueryArgument<BooleanGraphType>
                    {
                        Name = "includeDeprecated",
                        DefaultValue = false
                    }
                }),
                context =>
                {
                    if (context.Source is ObjectGraphType || context.Source is InterfaceGraphType)
                    {
                        var includeDeprecated = (bool)context.Arguments["includeDeprecated"];
                        var type = context.Source as GraphType;
                        return !includeDeprecated
                            ? type.Fields.Where(f => string.IsNullOrWhiteSpace(f.DeprecationReason))
                            : type.Fields;
                    }

                    return Enumerable.Empty<FieldType>();
                });
            Field<ListGraphType<NonNullGraphType<__Type>>>("interfaces", resolve: context =>
            {
                var type = context.Source as IImplementInterfaces;
                return type != null ? type.Interfaces : Enumerable.Empty<Type>();
            });
            Field<ListGraphType<NonNullGraphType<__Type>>>("possibleTypes", resolve: context =>
            {
                if (context.Source is InterfaceGraphType || context.Source is UnionGraphType)
                {
                    var type = (GraphType)context.Source;
                    return context.Schema.FindImplemenationsOf(type.GetType());
                }
                return Enumerable.Empty<GraphType>();
            });
            Field<ListGraphType<NonNullGraphType<__EnumValue>>>("enumValues", null,
                new QueryArguments(new[]
                {
                    new QueryArgument<BooleanGraphType>
                    {
                        Name = "includeDeprecated",
                        DefaultValue = false
                    }
                }),
                context =>
                {
                    var type = context.Source as EnumerationGraphType;
                    if (type != null)
                    {
                        var includeDeprecated = (bool)context.Arguments["includeDeprecated"];
                        return !includeDeprecated
                            ? type.Values.Where(e => !string.IsNullOrWhiteSpace(e.DeprecationReason))
                            : type.Values;
                    }

                    return Enumerable.Empty<EnumValue>();
                });
            Field<ListGraphType<NonNullGraphType<__InputValue>>>("inputFields", resolve: context =>
            {
                var type = context.Source as InputObjectGraphType;
                return type != null ? type.Fields : Enumerable.Empty<FieldType>();
            });
            Field<__Type>("ofType", resolve: context =>
            {
                if (context.Source is Type)
                {
                    var type = (Type) context.Source;
                    var genericType = type.GetGenericArguments()[0];
                    return genericType;
                }

                return null;
            });
        }

        public TypeKind KindForInstance(GraphType type)
        {
            if (type is EnumerationGraphType)
            {
                return TypeKind.ENUM;
            }
            if (type is ScalarGraphType)
            {
                return TypeKind.SCALAR;
            }
            if (type is ObjectGraphType)
            {
                return TypeKind.OBJECT;
            }
            if (type is InterfaceGraphType)
            {
                return TypeKind.INTERFACE;
            }
            if (type is UnionGraphType)
            {
                return TypeKind.UNION;
            }
            if (type is InputObjectGraphType)
            {
                return TypeKind.INPUT_OBJECT;
            }
            if (type is ListGraphType)
            {
                return TypeKind.LIST;
            }
            if (type is NonNullGraphType)
            {
                return TypeKind.NON_NULL;
            }

            throw new ExecutionError("Unkown kind of type: {0}".ToFormat(type));
        }

        public TypeKind KindForType(Type type)
        {
            if (typeof(EnumerationGraphType).IsAssignableFrom(type))
            {
                return TypeKind.ENUM;
            }
            if (typeof(ScalarGraphType).IsAssignableFrom(type))
            {
                return TypeKind.SCALAR;
            }
            if (typeof(ObjectGraphType).IsAssignableFrom(type))
            {
                return TypeKind.OBJECT;
            }
            if (typeof(InterfaceGraphType).IsAssignableFrom(type))
            {
                return TypeKind.INTERFACE;
            }
            if (typeof(UnionGraphType).IsAssignableFrom(type))
            {
                return TypeKind.UNION;
            }
            if (typeof (InputObjectGraphType).IsAssignableFrom(type))
            {
                return TypeKind.INPUT_OBJECT;
            }
            if (typeof (ListGraphType).IsAssignableFrom(type))
            {
                return TypeKind.LIST;
            }
            if (typeof(NonNullGraphType).IsAssignableFrom(type))
            {
                return TypeKind.NON_NULL;
            }

            throw new ExecutionError("Unkown kind of type: {0}".ToFormat(type));
        }
    }
}
