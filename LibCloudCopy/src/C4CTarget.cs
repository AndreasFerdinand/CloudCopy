namespace CloudCopy
{
    using System.Threading;
    using System.Threading.Tasks;

    public class C4CTarget : IRemoteResource
    {
        private readonly string entityObjectID;
        private readonly string entityID;
        private readonly IC4CQueryClient queryClient;
        private string typeCode = "10001";
        private IC4CEntityMapper entityMapper;

        public C4CTarget(string entityObjectID)
        {
            this.entityObjectID = entityObjectID;
        }

        public C4CTarget(string entityID, IC4CQueryClient queryClient)
        {
            this.entityID = entityID;
            this.queryClient = queryClient;
        }

        public string TypeCode { get => this.typeCode; set => this.typeCode = value; }

        public void SetC4CEntityMapper(IC4CEntityMapper c4cEntityMapper)
        {
            this.entityMapper = c4cEntityMapper;
        }

        public async Task<string> GetSubPathAsync()
        {
            return await this.GetSubPathAsync(new CancellationToken()).ConfigureAwait(false);
        }

        public async Task<string> GetSubPathAsync(CancellationToken cancellationToken)
        {
            string subPath;

            if (this.entityID != null)
            {
                var pathQuery = await this.queryClient.GetObjectIDFromUserFriendlyId(this.entityMapper.GetCollectionName(), this.entityID, this.entityMapper.GetNameOfUserFriendlyID(), cancellationToken);

                subPath = this.entityMapper.GetCollectionName() + "('" + pathQuery + "')/" + this.entityMapper.GetAttachmentCollectionName();
            }
            else
            {
                subPath = this.entityMapper.GetCollectionName() + "('" + this.entityObjectID + "')/" + this.entityMapper.GetAttachmentCollectionName();
            }

            return subPath;
        }
    }
}