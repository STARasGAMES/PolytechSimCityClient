using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class City
{
    #region Server data
    public string name { get; set; }
    public string _fields { get; set; }
    #endregion

    private const char delimiter = ';';
    
    public string[] Fields
    {
        get
        {
            string[] res = _fields.Split(delimiter);
            Assert.AreEqual(64, res.Length);
            return res;
        }
    }
}
