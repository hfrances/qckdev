#if PORTABLE // EXCLUDE.
#else

using System;

namespace qckdev
{

    /// <summary>
    /// Wraps <see cref="System.AppDomain"/> object for <see cref="IDisposable"/> implementation.
    /// </summary>
    public class AppDomainWrapper : IDisposable
    {

#region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainWrapper"/> class.
        /// </summary>
        /// <param name="appDomain">The application domain wrapped in this <see cref="AppDomainWrapper"/>.</param>
        public AppDomainWrapper(AppDomain appDomain)
        {
            this.AppDomain = appDomain;
        }

#endregion


#region properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="AppDomain"/> has been unloaded.
        /// </summary>
        public bool IsDisposed => disposedValue;

        /// <summary>
        /// Gets the application domain wrapped in this <see cref="AppDomainWrapper"/>.
        /// </summary>
        public AppDomain AppDomain { get; private set; }

#endregion


#region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Dispose managed state (managed objects).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                AppDomain.Unload(this.AppDomain);
                this.AppDomain = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="AppDomainWrapper"/>.
        /// </summary>
        ~AppDomainWrapper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // The finalizer is overridden.
        }

#endregion

    }
}

#endif