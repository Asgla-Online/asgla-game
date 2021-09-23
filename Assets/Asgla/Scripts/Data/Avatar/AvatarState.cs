using System;

namespace Asgla.Data.Avatar {

    [Serializable]
    public enum AvatarState : int {
        NONE = -1,
        NORMAL = 0,
        COMBAT = 1,
        DEAD = 2
    }

}
