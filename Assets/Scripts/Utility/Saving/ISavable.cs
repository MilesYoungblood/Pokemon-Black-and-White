﻿namespace Scripts.Utility
{
    public interface ISavable
    {
        object CaptureState();

        void RestoreState(object state);
    }
}
