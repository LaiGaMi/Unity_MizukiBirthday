using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace EasyTransition
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private GameObject transitionTemplate;

        private bool runningTransition;

        public UnityAction onTransitionBegin;
        public UnityAction onTransitionCutPointReached;
        public UnityAction onTransitionEnd;

        private static TransitionManager instance;


        // =========================================================
        // 初始化
        // =========================================================

        private void Awake()
        {
            instance = this;
        }


        // =========================================================
        // 取得 Instance
        // =========================================================

        public static TransitionManager Instance()
        {
            if (instance == null)
            {
                Debug.LogError(
                    "You tried to access the instance before it exists."
                );
            }

            return instance;
        }


        // =========================================================
        // 不切換場景的 Transition
        // =========================================================

        /// <summary>
        /// Starts a transition without loading a new level.
        /// </summary>
        /// <param name="transition">
        /// The settings of the transition you want to use.
        /// </param>
        /// <param name="startDelay">
        /// The delay before the transition starts.
        /// </param>

        public void Transition(
            TransitionSettings transition,
            float startDelay
        )
        {
            if (transition == null || runningTransition)
            {
                Debug.LogError(
                    "You have to assing a transition."
                );

                return;
            }

            runningTransition = true;

            StartCoroutine(
                Timer(
                    startDelay,
                    transition
                )
            );
        }


        // =========================================================
        // 使用場景名稱切換場景
        // =========================================================

        /// <summary>
        /// Loads the new Scene with a transition.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene you want to load.
        /// </param>
        /// <param name="transition">
        /// The settings of the transition you want to use
        /// to load your new scene.
        /// </param>
        /// <param name="startDelay">
        /// The delay before the transition starts.
        /// </param>

        public void Transition(
            string sceneName,
            TransitionSettings transition,
            float startDelay
        )
        {
            if (transition == null || runningTransition)
            {
                Debug.LogError(
                    "You have to assing a transition."
                );

                return;
            }

            runningTransition = true;

            StartCoroutine(
                Timer(
                    sceneName,
                    startDelay,
                    transition
                )
            );
        }


        // =========================================================
        // 使用場景 Index 切換場景
        // =========================================================

        /// <summary>
        /// Loads the new Scene with a transition.
        /// </summary>
        /// <param name="sceneIndex">
        /// The index of the scene you want to load.
        /// </param>
        /// <param name="transition">
        /// The settings of the transition you want to use
        /// to load your new scene.
        /// </param>
        /// <param name="startDelay">
        /// The delay before the transition starts.
        /// </param>

        public void Transition(
            int sceneIndex,
            TransitionSettings transition,
            float startDelay
        )
        {
            if (transition == null || runningTransition)
            {
                Debug.LogError(
                    "You have to assing a transition."
                );

                return;
            }

            runningTransition = true;

            StartCoroutine(
                Timer(
                    sceneIndex,
                    startDelay,
                    transition
                )
            );
        }


        // =========================================================
        // 取得場景 Index
        // =========================================================

        /// <summary>
        /// Gets the index of a scene from its name.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene you want to get the index of.
        /// </param>

        int GetSceneIndex(string sceneName)
        {
            return SceneManager.GetSceneByName(
                sceneName
            ).buildIndex;
        }


        // =========================================================
        // Transition：使用場景名稱
        // =========================================================

        IEnumerator Timer(
            string sceneName,
            float startDelay,
            TransitionSettings transitionSettings
        )
        {
            // 使用 RealTime
            // 不受 Time.timeScale = 0 影響
            yield return new WaitForSecondsRealtime(
                startDelay
            );


            // -----------------------------------------------------
            // Transition 開始
            // -----------------------------------------------------

            onTransitionBegin?.Invoke();


            // -----------------------------------------------------
            // 建立 Transition
            // -----------------------------------------------------

            GameObject template =
                Instantiate(
                    transitionTemplate
                ) as GameObject;


            template
                .GetComponent<Transition>()
                .transitionSettings =
                transitionSettings;


            // -----------------------------------------------------
            // 計算 Transition 時間
            // -----------------------------------------------------

            float transitionTime =
                transitionSettings.transitionTime;


            if (transitionSettings.autoAdjustTransitionTime)
            {
                transitionTime =
                    transitionTime /
                    transitionSettings.transitionSpeed;
            }


            // -----------------------------------------------------
            // 等待 Transition 到達切換點
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionTime
            );


            // -----------------------------------------------------
            // Transition Cut Point
            // -----------------------------------------------------

            onTransitionCutPointReached?.Invoke();


            // -----------------------------------------------------
            // 載入場景
            // -----------------------------------------------------

            SceneManager.LoadScene(
                sceneName
            );


            // -----------------------------------------------------
            // 等待 Transition 完成
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionSettings.destroyTime
            );


            onTransitionEnd?.Invoke();

            runningTransition = false;
        }


        // =========================================================
        // Transition：使用場景 Index
        // =========================================================

        IEnumerator Timer(
            int sceneIndex,
            float startDelay,
            TransitionSettings transitionSettings
        )
        {
            // 使用 RealTime
            yield return new WaitForSecondsRealtime(
                startDelay
            );


            // -----------------------------------------------------
            // Transition 開始
            // -----------------------------------------------------

            onTransitionBegin?.Invoke();


            // -----------------------------------------------------
            // 建立 Transition
            // -----------------------------------------------------

            GameObject template =
                Instantiate(
                    transitionTemplate
                ) as GameObject;


            template
                .GetComponent<Transition>()
                .transitionSettings =
                transitionSettings;


            // -----------------------------------------------------
            // 計算 Transition 時間
            // -----------------------------------------------------

            float transitionTime =
                transitionSettings.transitionTime;


            if (transitionSettings.autoAdjustTransitionTime)
            {
                transitionTime =
                    transitionTime /
                    transitionSettings.transitionSpeed;
            }


            // -----------------------------------------------------
            // 等待 Transition 到達切換點
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionTime
            );


            // -----------------------------------------------------
            // Transition Cut Point
            // -----------------------------------------------------

            onTransitionCutPointReached?.Invoke();


            // -----------------------------------------------------
            // 載入場景
            // -----------------------------------------------------

            SceneManager.LoadScene(
                sceneIndex
            );


            // -----------------------------------------------------
            // 等待 Transition 完成
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionSettings.destroyTime
            );


            onTransitionEnd?.Invoke();

            runningTransition = false;
        }


        // =========================================================
        // Transition：不切換場景
        // =========================================================

        IEnumerator Timer(
            float delay,
            TransitionSettings transitionSettings
        )
        {
            // 使用 RealTime
            yield return new WaitForSecondsRealtime(
                delay
            );


            // -----------------------------------------------------
            // Transition 開始
            // -----------------------------------------------------

            onTransitionBegin?.Invoke();


            // -----------------------------------------------------
            // 建立 Transition
            // -----------------------------------------------------

            GameObject template =
                Instantiate(
                    transitionTemplate
                ) as GameObject;


            template
                .GetComponent<Transition>()
                .transitionSettings =
                transitionSettings;


            // -----------------------------------------------------
            // 計算 Transition 時間
            // -----------------------------------------------------

            float transitionTime =
                transitionSettings.transitionTime;


            if (transitionSettings.autoAdjustTransitionTime)
            {
                transitionTime =
                    transitionTime /
                    transitionSettings.transitionSpeed;
            }


            // -----------------------------------------------------
            // 等待 Transition 到達切換點
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionTime
            );


            // -----------------------------------------------------
            // Transition Cut Point
            // -----------------------------------------------------

            onTransitionCutPointReached?.Invoke();


            // -----------------------------------------------------
            // 對目前場景執行 Transition
            // -----------------------------------------------------

            template
                .GetComponent<Transition>()
                .OnSceneLoad(
                    SceneManager.GetActiveScene(),
                    LoadSceneMode.Single
                );


            // -----------------------------------------------------
            // 等待 Transition 完成
            // -----------------------------------------------------

            yield return new WaitForSecondsRealtime(
                transitionSettings.destroyTime
            );


            onTransitionEnd?.Invoke();

            runningTransition = false;
        }


        // =========================================================
        // 檢查 Transition Manager
        // =========================================================

        private IEnumerator Start()
        {
            while (this.gameObject.activeInHierarchy)
            {
                // -------------------------------------------------
                // Unity 新版 API
                //
                // FindObjectsOfType 已經被棄用
                //
                // FindObjectsByType 可以指定：
                // 1. 是否包含 inactive 物件
                // 2. 是否排序
                // -------------------------------------------------

                TransitionManager[] managers =
                    Object.FindObjectsByType<TransitionManager>(
                        FindObjectsInactive.Include,
                        FindObjectsSortMode.None
                    );


                int managerCount =
                    managers.Length;


                // -------------------------------------------------
                // 檢查是否有多個 Transition Manager
                // -------------------------------------------------

                if (managerCount > 1)
                {
                    Debug.LogError(
                        "There are " +
                        managerCount.ToString() +
                        " Transition Managers in your scene. " +
                        "Please ensure there is only one " +
                        "Transition Manager in your scene " +
                        "or overlapping transitions may occur."
                    );
                }


                // -------------------------------------------------
                // 每秒檢查一次
                // -------------------------------------------------

                yield return new WaitForSecondsRealtime(
                    1f
                );
            }
        }
    }
}