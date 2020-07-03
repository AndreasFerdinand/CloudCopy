namespace CloudCopy
{
    using System;

    public static class TargetFactory
    {
        public static IRemoteResource CreateC4CTarget(string entityName, string entityObjectID)
        {
            return CreateC4CTarget(entityName, entityObjectID, "10001");
        }

        public static IRemoteResource CreateC4CTarget(string entityName, string entityObjectID, string typeCode)
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityObjectID);

            c4cTarget.SetC4CEntityMapper(c4cEntityMapper);

            c4cTarget.TypeCode = typeCode;

            return c4cTarget;
        }

        public static IRemoteResource CreateC4CTarget(string entityName, string entityID, IC4CQueryClient queryClient)
        {
            return CreateC4CTarget(entityName, entityID, queryClient, "10001");
        }

        public static IRemoteResource CreateC4CTarget(string entityName, string entityID, IC4CQueryClient queryClient, string typeCode)
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityID, queryClient);

            c4cTarget.SetC4CEntityMapper(c4cEntityMapper);

            c4cTarget.TypeCode = typeCode;

            return c4cTarget;
        }
    }
}