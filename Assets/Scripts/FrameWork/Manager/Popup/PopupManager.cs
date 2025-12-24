using System;
using System.Collections;
using UnityEngine;


namespace Assets.Scripts.FrameWork.Manager.Popup
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager s_instPopupMan;

        public static bool gReleaseResource = true;

        [SerializeField]
        private GameObject m_canvasNotForm;


        public static Transform GetParentTrans(bool isDoNot)
        {
            if (isDoNot == false)
                return GameObject.Find("PopupForm").transform;
            else
                return s_instPopupMan.m_canvasNotForm.transform;
        }

        public void Delay()
        {

        }
                
        static public PopupManager GetInstance()
        {
            if (s_instPopupMan == null)
                return null;

            return s_instPopupMan;
        }

        static public int ActingPopupCount
        {
            get
            {
                if (s_instPopupMan != null)
                    return s_instPopupMan.GetActingPopupCount();

                return 0;
            }
        }

        static public int PopupCount
        {
            get
            {
                if (s_instPopupMan != null)
                    return s_instPopupMan.GetActingPopupCount();

                return 0;
            }
        }

        private void Awake()
        {
            s_instPopupMan = this;

            DontDestroyOnLoad(gameObject);
        }

        //팝업추가
        public bool AddPopup(GameObject popup, bool isDoNot)
        {
            if (popup != null)
            {
                Transform canvasTrans = GetParentTrans(isDoNot);

                popup.transform.SetParent(canvasTrans);
                popup.transform.localScale = Vector3.one;
                popup.transform.localPosition = Vector3.zero;
                                
                //popup에 CanvasGroup Component를 추가 팝업 alpha fadeout 처리를 위하여
                if (popup.TryGetComponent<CanvasGroup>(out _) == false)
                    popup.AddComponent<CanvasGroup>();

                return true;
            }

            return false;
        }

        //팝업닫기
        public float ClosePopup(GameObject popup)
        {
            if (popup != null)
            {
                if (popup.TryGetComponent<CanvasGroup>(out _) == false)
                    return 0f;
                                
                bool bFadeOutAndDestroy = false;
                float fDelayDestory = 0f; 
                
                if (false == bFadeOutAndDestroy)
                    Destroy(popup.transform.gameObject);

                return fDelayDestory;
            }

            return 0f;
        }

        public IEnumerator ReleaseResource()
        {
            yield return null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Resources.UnloadUnusedAssets();
        }

        public int GetActingPopupCount()
        {
            int nCnt = 0;

            var canvasTrans = GetParentTrans(false);
            if (canvasTrans != null)
            {
                foreach (Transform t in canvasTrans)
                {
                    if (t.gameObject.activeSelf == true)
                        nCnt++;
                }
            }

            return nCnt;
        }

        public bool IsRunPopup(string popupName, bool isNot = false)
        {
            var canvasTrans = GetParentTrans(isNot);
            if (canvasTrans == null)
                return false;

            return canvasTrans.Find(popupName) != null;
        }

        public bool OnEscapeKey()
        {
            if (gameObject.activeInHierarchy)
            {
                var canvasTrans = GetParentTrans(false);
                if (canvasTrans != null)
                {
                    foreach (Transform topRoot in canvasTrans)
                        ClosePopup(topRoot.gameObject);
                }
            }

            return false;
        }

        public void AllClosePopup()
        {
            var canvasTrans = GetParentTrans(false);
            if (canvasTrans == null)
                return;

            foreach (Transform topRoot in canvasTrans)
                ClosePopup(topRoot.gameObject);
        }


        public void SelectClosePopup(string popupName)
        {
            var canvasTrans = GetParentTrans(false);
            if (canvasTrans == null)
                return;

            foreach (Transform topRoot in canvasTrans)
            {
                if (topRoot.gameObject.name.Equals(popupName))
                {
                    ClosePopup(topRoot.gameObject);
                    break;
                }
            }
        }

        public void SelectClosePopupDoNot(string popupName)
        {
            var canvasTrans = GetParentTrans(true);
            if (canvasTrans == null)
                return;

            foreach (Transform topRoot in canvasTrans)
            {
                if (topRoot.gameObject.name.Equals(popupName))
                {
                    Destroy(topRoot.gameObject);
                    break;
                }
            }
        }

        public void IsActNext()
        {

        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                OnEscapeKey();
        }
    };
};

