using System;
using System.Threading.Tasks;

namespace CloudCopy
{
    class C4CTarget : IRemoteResource
    {
        string _EntityObjectID;
        string _EntityID;
        string _TypeCode = "10001";

        IC4CEntityMapper _C4CEntityMapper;

        IC4CQueryClient _QueryClient;

        public string TypeCode { get => _TypeCode; set => _TypeCode = value; }

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

        public async Task<string> getSubPathAsync()
        {
            string subPath;

            if ( _EntityID != null )
            {
                var PathQuery = await _QueryClient.getObjectIDFromID(_C4CEntityMapper.getCollectionName(),_EntityID, _C4CEntityMapper.getHumanReadableIDName());

                subPath = _C4CEntityMapper.getCollectionName() + "('" + PathQuery + "')/" + _C4CEntityMapper.getAttachmentCollectionName();
            }
            else
            {
                subPath = _C4CEntityMapper.getCollectionName() + "('" + _EntityObjectID + "')/" + _C4CEntityMapper.getAttachmentCollectionName();
            }

            return subPath;
        }
    }

}