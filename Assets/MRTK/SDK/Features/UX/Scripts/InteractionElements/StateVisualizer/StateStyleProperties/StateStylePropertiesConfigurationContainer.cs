using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateStylePropertiesConfigurationContainer : ScriptableObject
    {
        
        public string StateName;

  
        public GameObject Target;

      
        public List<StateStylePropertyConfiguration> StateStylePropList;

    }
}