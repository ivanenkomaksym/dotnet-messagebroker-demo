using NetArchTest.Rules;

namespace Architecture.Tests.Rules
{
    public class InterfacesShouldStartWithIRule : ICustomRule
    {
        public bool MeetsRule(Mono.Cecil.TypeDefinition type)
        {
            if (!type.IsInterface)
                return true;

            if (!type.Name.StartsWith('I'))
                return false;

            return true;
        }
    }
}