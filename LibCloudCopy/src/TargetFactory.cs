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

        public static IRemoteResource CreateC4CTarget(string targetEntity, IC4CQueryClient queryClient)
        {
            return CreateC4CTarget(targetEntity, queryClient, string.Empty);
        }

        public static IRemoteResource CreateC4CTarget(string targetEntity, IC4CQueryClient queryClient, string typeCode)
        {
            string[] target = targetEntity.Split(":");

            if (target.Length != 2)
            {
                //TODO RAISE EXCEPTION
            }

            string entityName = target[0];
            string entityID = target[1];

            if (entityID[0] == '#')
            {
                return CreateC4CTarget(entityName, entityID.Substring(1), queryClient, typeCode);
            }
            else
            {
                return CreateC4CTarget(entityName, entityID, typeCode);
            }
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