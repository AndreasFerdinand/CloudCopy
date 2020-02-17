using System;
using System.Threading.Tasks;

namespace CloudCopy
{
    class C4CTarget : IRemoteResource
    {
        string _EntityObjectID;
        string _EntityID;

        IC4CEntityMapper _C4CEntityMapper;

        IC4CQueryClient _QueryClient;

        public void setC4CEntityMapper(IC4CEntityMapper c4cEntityMapper)
        {
            _C4CEntityMapper = c4cEntityMapper;
        }

        public C4CTarget(string EntityObjectID)
        {
            _EntityObjectID = EntityObjectID;
        }

        public C4CTarget(string entityID, IC4CQueryClient queryClient)
        {
            _EntityID = entityID;
            _QueryClient = queryClient;
        }

        public string getSubPath()
        {
            string subPath;

            if ( _EntityID != null )
            {
                Task<string> PathQuery;
                PathQuery = _QueryClient.getObjectIDFromID(_C4CEntityMapper.getCollectionName(),_EntityID, _C4CEntityMapper.getHumanReadableIDName());

                //subPath = _ParentCollectionName + "('" + PathQuery.Result + "')/" + _AttachmentCollectionName;
                subPath = _C4CEntityMapper.getCollectionName() + "('" + PathQuery.Result + "')/" + _C4CEntityMapper.getAttachmentCollectionName();
            }
            else
            {
                subPath = _C4CEntityMapper.getCollectionName() + "('" + _EntityObjectID + "')/" + _C4CEntityMapper.getAttachmentCollectionName();
            }

            
            return subPath;
        }
    }

}