using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentStructure : MonoBehaviour
{
    [SerializeField]
    Text nickname;
    [SerializeField]
    Text comment;

    public void SetCommentInfo(string nickname, string comment)
    {
        this.nickname.text = nickname;
        this.comment.text = comment;
    }
}
