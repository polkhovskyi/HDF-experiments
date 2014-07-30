using System;

namespace HDF_experiments.HDF
{
    public class HdfWrapper<TId> : IDisposable
    {
        private readonly Action<TId> _destructorAction;
        private readonly TId _id;

        public HdfWrapper(Func<TId> idFunc, Action<TId> destructorAction)
        {
            _id = idFunc();
            _destructorAction = destructorAction;
        }

        public TId Id
        {
            get { return _id; }
        }

        public void Dispose()
        {
            _destructorAction(_id);
        }
    }
}