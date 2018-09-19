using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using Soomla.Store.Charsoo;
using UnityEngine;

namespace Soomla.Store.Charsoo
{
    public class BazaarInitializer : MonoBehaviour
    {

        private static CharsooStoreEventHandler _handler;
        //public SceneLoader SceneLoad;
        // Use this for initialization
        public void Start()
        {
            Initialize();
        }

        // UpdateData is called once per frame
        void Update()
        {

        }

        public void Initialize()
        {
            _handler = new CharsooStoreEventHandler();
            CharsooStoreAsset storeAsset = new CharsooStoreAsset();
            SoomlaStore.Initialize(storeAsset);
        }
    }
}
