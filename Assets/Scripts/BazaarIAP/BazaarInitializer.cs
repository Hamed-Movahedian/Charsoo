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
        // Use this for initialization
        void Start()
        {
            _handler=new CharsooStoreEventHandler();
            bool b = SoomlaStore.Initialize(new CharsooStoreAsset());
            Debug.Log(b);
        }

        // UpdateData is called once per frame
        void Update()
        {

        }
    }
}
