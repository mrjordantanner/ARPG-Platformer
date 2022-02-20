using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class PoolController : MonoBehaviour
    {
        //[Header("Enemy Pools")]
        //public GameObject Spider1;
        //public GameObject Spider2, Orb, Bullet, ExplosiveBeam;

        //[HideInInspector]
        //public SimplePool.Pool spiderPool1, spiderPool2, enemyOrbPool, enemyBulletPool, explosiveBeamPool;

        #region Singleton
        public static PoolController Instance;
        private void Awake()
        {
            if (Application.isEditor)
                Instance = this;
            else
            {
                if (Instance == null)
                    Instance = this;
                else
                {
                    Destroy(gameObject);
                    return;
                }

                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public void CreateEnemyPools()
        {
            //spiderPool1 = new SimplePool.Pool(Spider1, 150);
            //spiderPool2 = new SimplePool.Pool(Spider2, 150);
            //enemyOrbPool = new SimplePool.Pool(Orb, 500);
            //enemyBulletPool = new SimplePool.Pool(Bullet, 500);
            //explosiveBeamPool = new SimplePool.Pool(ExplosiveBeam, 500);
        }


    }
}
