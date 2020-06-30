namespace CloudCopy
{
    using System;

    public class TargetFactory
    {
        public static IRemoteResource CreateC4CTarget(string entityName, string entityObjectID, string typeCode = "10001")
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityObjectID);

            c4cTarget.SetC4CEntityMapper(c4cEntityMapper);

            c4cTarget.TypeCode = typeCode;

            return c4cTarget;
        }

        public static IRemoteResource CreateC4CTarget(string entityName, string entityID, IC4CQueryClient queryClient, string typeCode = "10001")
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityID, queryClient);

            c4cTarget.SetC4CEntityMapper(c4cEntityMapper);

            c4cTarget.TypeCode = typeCode;

            return c4cTarget;
        }
    }
}