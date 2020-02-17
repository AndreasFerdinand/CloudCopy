using System;

namespace CloudCopy
{
    class TargetFactory
    {
        public static IRemoteResource createC4CTarget(string entityName, string entityObjectID)
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityObjectID);

            c4cTarget.setC4CEntityMapper(c4cEntityMapper);

            return c4cTarget;
        }

        public static IRemoteResource createC4CTarget(string entityName, string entityID, IC4CQueryClient queryClient)
        {
            IC4CEntityMapper c4cEntityMapper = new C4CEntityMapper(entityName);

            C4CTarget c4cTarget = new C4CTarget(entityID,queryClient);

            c4cTarget.setC4CEntityMapper(c4cEntityMapper);

            return c4cTarget;
        }
    }
}