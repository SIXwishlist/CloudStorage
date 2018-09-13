using System.Threading;
using System.Web;
using Microsoft.Identity.Client;
using OneDriveFinal.Models;
using System.Linq;
using System.Data.Entity;
using System;

namespace OneDriveFinal.TokenStorage
{// Store the user's token information.
    // Store the user's token information.
    public class EFTokenCache
    {
        private static ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        string UserId = string.Empty;
        string CacheId = string.Empty;
        //HttpContextBase httpContext = null;

        TokenCache cache = new TokenCache();

        private WebAppContext _context = new WebAppContext();

        public EFTokenCache(string userId, HttpContextBase httpcontext)
        {
            // not object, we want the SUB
            UserId = userId;
            CacheId = UserId + "_TokenCache";
            //httpContext = httpcontext;
            Load();
        }

        public TokenCache GetMsalCacheInstance()
        {
            cache.SetBeforeAccess(BeforeAccessNotification);
            cache.SetAfterAccess(AfterAccessNotification);
            Load();
            return cache;
        }

        //public void SaveUserStateValue(string state)
        //{
        //    SessionLock.EnterWriteLock();
        //    //httpContext.Session[CacheId + "_state"] = state;

        //    //===============EF==================

        //    // Get userCache from DB
        //    PerWebUserCache userCache = _context.PerWebUserCaches.FirstOrDefault(c => c.WebUserUniqueId == UserId);

        //    // Update existing record
        //    if (userCache != null)
        //    {
        //        userCache.UserState = state;
        //    }
        //    // Create a new record
        //    else
        //    {
        //        userCache = new PerWebUserCache()
        //        {
        //            WebUserUniqueId = UserId,
        //            UserState = state
        //        };
        //    }

        //    // Update the cache to DB               
        //    _context.Entry(userCache).State = userCache.Id == 0 ? EntityState.Added : EntityState.Modified;
        //    _context.SaveChanges();

        //    // =====================================

        //    SessionLock.ExitWriteLock();
        //}
        //public string ReadUserStateValue()
        //{
        //    string state = string.Empty;
        //    SessionLock.EnterReadLock();
        //    //state = (string)httpContext.Session[CacheId + "_state"];

        //    //===============EF==================

        //    // Get userCache from DB
        //    PerWebUserCache userCache = _context.PerWebUserCaches.FirstOrDefault(c => c.WebUserUniqueId == UserId);

        //    // Update existing record
        //    if (userCache != null)
        //    {
        //        state = userCache.UserState;
        //    }


        //    // =====================================


        //    SessionLock.ExitReadLock();
        //    return state;
        //}
        public void Load()
        {
            SessionLock.EnterReadLock();
            //cache.Deserialize((byte[])httpContext.Session[CacheId]);

            //===============EF==================

            // Get userCache from DB
            PerWebUserCache userCache = _context.PerWebUserCaches.FirstOrDefault(c => c.WebUserUniqueId.Equals(UserId));

            // Update existing record
            if (userCache != null)
            {
                cache.Deserialize(userCache.CacheBits);
            }


            // =====================================

            SessionLock.ExitReadLock();
        }

        public void Persist()
        {
            SessionLock.EnterWriteLock();

            // Optimistically set HasStateChanged to false. We need to do it early to avoid losing changes made by a concurrent thread.
            cache.HasStateChanged = false;

            // Reflect changes in the persistent store
            //httpContext.Session[CacheId] = cache.Serialize();

            //===============EF==================

            // Get userCache from DB
            PerWebUserCache userCache = _context.PerWebUserCaches.FirstOrDefault(c => c.WebUserUniqueId.Equals(UserId));

            // Update existing record
            if (userCache != null)
            {
                userCache.CacheBits = cache.Serialize();
                userCache.LastWrite = DateTime.Now;
            }
            // Create a new record
            else
            {
                userCache = new PerWebUserCache()
                {
                    WebUserUniqueId = UserId,
                    CacheBits = cache.Serialize(),
                    LastWrite = DateTime.Now
                };
            }

            // Update the cache to DB               
            _context.Entry(userCache).State = userCache.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();

            // =====================================

            SessionLock.ExitWriteLock();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (cache.HasStateChanged)
            {
                Persist();
            }
        }

    }
}