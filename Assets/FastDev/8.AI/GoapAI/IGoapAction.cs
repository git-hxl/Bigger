using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev
{
    public interface IGoapAction
    {
        HashSet<KeyValuePair<string, object>> Preconditions { get; }
        HashSet<KeyValuePair<string, object>> Effects { get; }

        int Cost { get; }

        GameObject Target { get; }

        bool IsDone { get; }

        void Reset();
        bool RequireInRange();
        bool IsInRange(Vector3 pos);
        /// <summary>
        /// ����Ƿ�����ִ������
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool CheckProceduralPrecondition(IGoapAgent goapAgent);
        void Start();
        bool Run(IGoapAgent goapAgent);
    }
}