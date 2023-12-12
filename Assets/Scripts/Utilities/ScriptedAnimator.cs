using Enums;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScriptedAnimator : MonoBehaviour
{
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private ParticleSystem particles;
    [Space]
    [SerializeField] private Sprite[] sprites;
    [Space(10)]
    [Tooltip("If left as -1 then its not going to do damage")]
    [HideInInspector] public List<int> eventFrame;
    [HideInInspector] public List<FrameEvent> frameEvents;

    private int damagingFramesCount;
    private int spritesLenght;
    private SpriteRenderer spriteRenderer;
    protected Entity m_entity;

    private Vector3 originalPosition;
    private void Awake()
    {
        originalPosition = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesLenght = sprites.Length;
        damagingFramesCount = eventFrame.Count;
        m_entity = GetComponentInParent<Entity>();
    }
    public void PlayAnimation()
    {
        StartCoroutine(AnimationCorutine());
    }

    private IEnumerator AnimationCorutine()
    {
        Vector3 position = transform.position;
        transform.position = position + offset;
        int frame = 0;
        int currentDamagingFrame = 0;
        while (frame < spritesLenght)
        {
            spriteRenderer.sprite = sprites[frame];
            if (damagingFramesCount >= 0)
            {
                if (frame == eventFrame[currentDamagingFrame])
                {
                    switch (frameEvents[currentDamagingFrame])
                    {
                        case FrameEvent.damagePlayer:
                            m_entity.AttackEntity();
                            break;
                        case FrameEvent.playParticles:
                            if (particles) particles.Play();
                            break;
                        case FrameEvent.stopParticles:
                            if (particles) particles.Stop();
                            break;
                    }
  
                    if (currentDamagingFrame < damagingFramesCount - 1) currentDamagingFrame++;
                    
                }
            }
            frame++;
            // if (frame == dealDamageOnFrame) m_entity.AttackEntity();
            yield return new WaitForSeconds(frameRate);
        }
        spriteRenderer.sprite = null ;
        if (particles) particles.Stop();
        transform.position = position;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptedAnimator))]
    public class ScriptedAnimatorGUI : Editor
    {
        protected ScriptedAnimator animator;

        private const int LABEL_WIDTH = 70;
        private const int FIELD_WIDTH = 50;
        private const int ENUM_FIELD_WIDTH = 130;

        protected static bool ShowAnimationEvents = false;
        private void OnEnable()
        {
            animator = (ScriptedAnimator)target;
            EditorUtility.SetDirty(animator);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            ShowAnimationEvents = EditorGUILayout.Foldout(ShowAnimationEvents, "Animation Events", true);

            if (ShowAnimationEvents)
            {
                EditorGUI.indentLevel++;
                List<int> eventList = animator.eventFrame;
                int size = Mathf.Max(0, EditorGUILayout.IntField("Size", eventList.Count));

                List<FrameEvent> frameEventList = animator.frameEvents;
                int frameSize = size;

                while (size > eventList.Count)
                {
                    eventList.Add(0);
                    frameEventList.Add(0);
                }

                while (size < eventList.Count)
                {
                    eventList.RemoveAt(eventList.Count - 1);
                    frameEventList.RemoveAt(frameEventList.Count - 1);
                }
    
                for (int i = 0; i < size; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("In Frame:", GUILayout.MaxWidth(LABEL_WIDTH));
                    eventList[i] = EditorGUILayout.IntField(eventList[i],GUILayout.MaxWidth(FIELD_WIDTH));

                    frameEventList[i] = (FrameEvent)EditorGUILayout.EnumPopup(frameEventList[i], GUILayout.MaxWidth(ENUM_FIELD_WIDTH));
                    EditorGUILayout.EndHorizontal();
                }

                //for (int i = 0; i < frameSize; i++)
                //{
                //    frameEventList[i] = (FrameEvent)EditorGUILayout.EnumPopup("Frame Event", frameEventList[i], GUILayout.MaxWidth(FIELD_WIDTH));
                //}


                EditorGUI.indentLevel--;
            }
            //ShowSkillList = EditorGUILayout.Foldout(ShowSkillList, "Skill List", true);

            //if (ShowSkillList)
            //{

            //    EditorGUI.indentLevel++;
            //    List<int> list = animator.eventFrame;
            //    int size = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));

            //    while (size > list.Count)
            //    {
            //        list.Add(0);
            //    }

            //    while (size < list.Count)
            //    {
            //        list.RemoveAt(list.Count - 1);
            //    }

            //    for (int i = 0; i < list.Count; i++)
            //    {
            //       // list[i] = (SkillEnemy)EditorGUILayout.IntField(i, list[i], typeof(SkillEnemy), false);
            //    }

            //    EditorGUI.indentLevel--;

        }
    }
#endif
}
