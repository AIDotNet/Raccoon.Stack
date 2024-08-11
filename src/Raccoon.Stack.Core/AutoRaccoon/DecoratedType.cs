using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Raccoon.Stack.Core.AutoRaccoon;

public class DecoratedType : Type
{
    public DecoratedType(Type type)
    {
        ProxiedType = type;
    }

    private Type ProxiedType { get; }
    public override string? Namespace => ProxiedType.Namespace;
    public override string? AssemblyQualifiedName => ProxiedType.AssemblyQualifiedName;
    public override string? FullName => ProxiedType.FullName;
    public override Assembly Assembly => ProxiedType.Assembly;
    public override Module Module => ProxiedType.Module;
    public override Type? DeclaringType => ProxiedType.DeclaringType;
    public override MethodBase? DeclaringMethod => ProxiedType.DeclaringMethod;
    public override Type? ReflectedType => ProxiedType.ReflectedType;
    public override Type UnderlyingSystemType => ProxiedType.UnderlyingSystemType;

#if NETCOREAPP3_1_OR_GREATER
    public override bool IsTypeDefinition => ProxiedType.IsTypeDefinition;
#endif
    public override bool IsConstructedGenericType => ProxiedType.IsConstructedGenericType;
    public override bool IsGenericParameter => ProxiedType.IsGenericParameter;
    public override bool IsGenericType => ProxiedType.IsGenericType;
    public override bool IsGenericTypeDefinition => ProxiedType.IsGenericTypeDefinition;
    public override int GenericParameterPosition => ProxiedType.GenericParameterPosition;
    public override GenericParameterAttributes GenericParameterAttributes => ProxiedType.GenericParameterAttributes;
    public override bool IsEnum => ProxiedType.IsEnum;
#if NETCOREAPP3_1_OR_GREATER
    public override bool IsSignatureType => ProxiedType.IsSignatureType;
#endif
    public override bool IsSecurityCritical => ProxiedType.IsSecurityCritical;
    public override bool IsSecuritySafeCritical => ProxiedType.IsSecuritySafeCritical;
    public override bool IsSecurityTransparent => ProxiedType.IsSecurityTransparent;
    public override StructLayoutAttribute? StructLayoutAttribute => ProxiedType.StructLayoutAttribute;
    public override RuntimeTypeHandle TypeHandle => ProxiedType.TypeHandle;
    public override Guid GUID => ProxiedType.GUID;
    public override Type? BaseType => ProxiedType.BaseType;
    public override MemberTypes MemberType => ProxiedType.MemberType;
    public override string Name => $"{ProxiedType.Name}+Decorated";
    public override IEnumerable<CustomAttributeData> CustomAttributes => ProxiedType.CustomAttributes;
    public override int MetadataToken => ProxiedType.MetadataToken;

    // We use object reference equality here to ensure that only the decorating object can match.
    public override bool Equals(Type? o)
    {
        return ReferenceEquals(this, o);
    }

    public override bool Equals(object? o)
    {
        return ReferenceEquals(this, o);
    }

    public override int GetHashCode()
    {
        return ProxiedType.GetHashCode();
    }

    protected override bool IsArrayImpl()
    {
        return ProxiedType.HasElementType;
    }

    protected override bool IsByRefImpl()
    {
        return ProxiedType.IsByRef;
    }

    protected override bool IsPointerImpl()
    {
        return ProxiedType.IsPointer;
    }

    protected override bool HasElementTypeImpl()
    {
        return ProxiedType.HasElementType;
    }

    public override Type? GetElementType()
    {
        return ProxiedType.GetElementType();
    }

    public override int GetArrayRank()
    {
        return ProxiedType.GetArrayRank();
    }

    public override Type GetGenericTypeDefinition()
    {
        return ProxiedType.GetGenericTypeDefinition();
    }

    public override Type[] GetGenericArguments()
    {
        return ProxiedType.GetGenericArguments();
    }

    public override Type[] GetGenericParameterConstraints()
    {
        return ProxiedType.GetGenericParameterConstraints();
    }

    protected override TypeAttributes GetAttributeFlagsImpl()
    {
        return ProxiedType.Attributes;
    }

    protected override bool IsCOMObjectImpl()
    {
        return ProxiedType.IsCOMObject;
    }

    protected override bool IsContextfulImpl()
    {
        return ProxiedType.IsContextful;
    }

    protected override bool IsMarshalByRefImpl()
    {
        return ProxiedType.IsMarshalByRef;
    }

    protected override bool IsPrimitiveImpl()
    {
        return ProxiedType.IsPrimitive;
    }

    protected override bool IsValueTypeImpl()
    {
        return ProxiedType.IsValueType;
    }

    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder,
        CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
    {
        return ProxiedType.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
    }

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
        return ProxiedType.GetConstructors(bindingAttr);
    }

    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
    {
        return ProxiedType.GetEvent(name, bindingAttr);
    }

    public override EventInfo[] GetEvents()
    {
        return ProxiedType.GetEvents();
    }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
        return ProxiedType.GetEvents(bindingAttr);
    }

    public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
    {
        return ProxiedType.GetField(name, bindingAttr);
    }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
        return ProxiedType.GetFields(bindingAttr);
    }

    public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
    {
        return ProxiedType.GetMember(name, bindingAttr);
    }

    public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
    {
        return ProxiedType.GetMember(name, type, bindingAttr);
    }
#if NET6_0
    public override MemberInfo GetMemberWithSameMetadataDefinitionAs(MemberInfo member)
    {
        return ProxiedType.GetMemberWithSameMetadataDefinitionAs(member);
    }
#endif
    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
        return ProxiedType.GetMembers(bindingAttr);
    }

    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder,
        CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
    {
        return ProxiedType.GetMethod(name, bindingAttr, binder, callConvention, types!, modifiers);
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
        return ProxiedType.GetMethods(bindingAttr);
    }

    public override Type? GetNestedType(string name, BindingFlags bindingAttr)
    {
        return ProxiedType.GetNestedType(name, bindingAttr);
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
        return ProxiedType.GetNestedTypes(bindingAttr);
    }

    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder,
        Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
    {
        return ProxiedType.GetProperty(name, bindingAttr, binder, returnType, types!, modifiers);
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
        return ProxiedType.GetProperties(bindingAttr);
    }

    public override MemberInfo[] GetDefaultMembers()
    {
        return ProxiedType.GetDefaultMembers();
    }

    protected override TypeCode GetTypeCodeImpl()
    {
        return GetTypeCode(ProxiedType);
    }

    public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target,
        object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
    {
        return ProxiedType.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
    }

    public override Type? GetInterface(string name, bool ignoreCase)
    {
        return ProxiedType.GetInterface(name, ignoreCase);
    }

    public override Type[] GetInterfaces()
    {
        return ProxiedType.GetInterfaces();
    }

    public override InterfaceMapping GetInterfaceMap(Type interfaceType)
    {
        return ProxiedType.GetInterfaceMap(interfaceType);
    }

    public override bool IsInstanceOfType(object? o)
    {
        return ProxiedType.IsInstanceOfType(o);
    }

    public override bool IsEquivalentTo(Type? other)
    {
        return ProxiedType.IsEquivalentTo(other);
    }

    public override Type GetEnumUnderlyingType()
    {
        return ProxiedType.GetEnumUnderlyingType();
    }

    public override Array GetEnumValues()
    {
        return ProxiedType.GetEnumValues();
    }

    public override Type MakeArrayType()
    {
        return ProxiedType.MakeArrayType();
    }

    public override Type MakeArrayType(int rank)
    {
        return ProxiedType.MakeArrayType(rank);
    }

    public override Type MakeByRefType()
    {
        return ProxiedType.MakeByRefType();
    }

    public override Type MakeGenericType(params Type[] typeArguments)
    {
        return ProxiedType.MakeGenericType(typeArguments);
    }

    public override Type MakePointerType()
    {
        return ProxiedType.MakePointerType();
    }

    public override string ToString()
    {
        return "Type: " + Name;
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        return ProxiedType.GetCustomAttributes(inherit);
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        return ProxiedType.GetCustomAttributes(attributeType, inherit);
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        return ProxiedType.IsDefined(attributeType, inherit);
    }

    public override IList<CustomAttributeData> GetCustomAttributesData()
    {
        return ProxiedType.GetCustomAttributesData();
    }
#if NETCOREAPP3_1_OR_GREATER
    public override bool IsGenericTypeParameter => ProxiedType.IsGenericTypeParameter;
    public override bool IsGenericMethodParameter => ProxiedType.IsGenericMethodParameter;
#endif
#if NETCOREAPP3_1_OR_GREATER
    public override bool IsSZArray => ProxiedType.IsSZArray;
    public override bool IsVariableBoundArray => ProxiedType.IsVariableBoundArray;
    public override bool IsByRefLike => ProxiedType.IsByRefLike;
#endif
}