using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerSetting
{
    public static class PowerSettings
    {

        #region The group and subGroup GUID number.
        //SubGroup: Sleep
        private static Guid GUID_SLEEP_SUBGROUP = new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");
        private static Guid GUID_STANDBYIDLE = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");   //Sleep State
        private static Guid GUID_HIBERNATEIDLE = new Guid("9d7815a6-7ee4-497e-8888-515a05f02364"); //Hibernate State

        //SubGroup: control system power buttons
        private static Guid GUID_SYSTEM_BUTTON_SUBGROUP = new Guid("4f971e89-eebd-4455-a8de-9e59040e7347");
        private static Guid GUID_LIDCLOSE_ACTION = new Guid("5ca83367-6e45-459f-a27b-476b1d01c936");       //lid close action
        private static Guid GUID_LIDOPEN_ACTION = new Guid("99ff10e7-23b1-4c07-a9d1-5c3206d741b4");        //lid open action
        private static Guid GUID_POWERBUTTON_ACTION = new Guid("7648efa3-dd9c-4e3e-b566-50f929386280");    //power button press action

        //SubGroup: Display settings
        private static Guid GUID_VIDEO_SUBGROUP = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");
        private static Guid GUID_VIDEOIDLE = new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e"); // Screen display turn off.

        #endregion


        #region Get/Set Power setting Windows API
        [DllImport("powrprof.dll")]
        static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll")]
        static extern uint PowerReadACValue(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            ref int Type,
            ref int Buffer,
            ref uint BufferSize);

        [DllImport("powrprof.dll")]
        static extern uint PowerReadDCValue(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            ref int Type,
            ref int Buffer,
            ref uint BufferSize);


        [DllImport("powrprof.dll")]
        static extern uint PowerReadACValueIndex(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            ref int AcValueIndex
            );

        [DllImport("powrprof.dll")]
        static extern uint PowerReadDCValueIndex(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            ref int DcValueIndex
            );


        [DllImport("powrprof.dll")]
        static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid powerSchemeGuid);

        [DllImport("powrprof.dll")]
        static extern uint PowerWriteACValueIndex(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            int AcValueIndex);

        [DllImport("powrprof.dll")]
        static extern uint PowerWriteDCValueIndex(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            int DcValueIndex);

        #endregion


        #region 1. 获取进入睡眠/休眠的时间

        /// <summary>
        /// 获取接电源时，多久进入睡眠，即待机(standby)，单位为秒
        /// value = 0,表示从不休眠
        /// </summary>
        public static int ReadACSleepTimeOut()
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
            int type = 0;
            int value = 0;
            uint valueSize = 4u;

            uint errorRes = PowerReadACValue(IntPtr.Zero,
                ref activePolicyGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_STANDBYIDLE,
                ref type,
                ref value,
                ref valueSize);

            if (errorRes != 0)
            {
                Console.WriteLine("Read AC Sleep Time failed.");
            }

            return value;
        }

        /// <summary>
        /// 获取接电源时,多久后进入休眠，单位为秒.
        /// value = 0,表示从不休眠
        /// </summary>
        public static int ReadACHibernateTimeOut()
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
            int type = 0;
            int value = 0;
            uint valueSize = 4u;

            uint errorRes = PowerReadACValue(IntPtr.Zero,
                ref activePolicyGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_HIBERNATEIDLE,
                ref type,
                ref value,
                ref valueSize);

            //if(errorRes==0)
            //{
            //    MessageBox.Show($"AC Hibernate after {value} seconds.");
            //}

            return value;

            //uint resVal = 0;
            //if (PowerReadACValueIndex(IntPtr.Zero, ref activePolicyGuid, ref GUID_SLEEP_SUBGROUP, ref GUID_HIBERNATEIDLE, ref resVal) == 0)
            //{
            //    MessageBox.Show($"value={resVal}");
            //}
        }

        /// <summary>
        /// 获取使用电池时，多久进入睡眠，单位为秒
        /// </summary>
        public static int ReadDCSleepTimeOut()
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
            int type = 0;
            int value = 0;
            uint valueSize = 4u;

            uint errorRes = PowerReadDCValue(IntPtr.Zero,
                ref activePolicyGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_STANDBYIDLE,
                ref type,
                ref value,
                ref valueSize);

            //if (errorRes == 0)
            //{
            //    MessageBox.Show($"DC Sleep after {value} seconds.");
            //}

            return value;
        }

        /// <summary>
        /// 获取使用电池时，多久进入休眠，单位为秒
        /// </summary>
        public static int ReadDCHibernateTimeOut()
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
            int type = 0;
            int value = 0;
            uint valueSize = 4u;

            uint errorRes = PowerReadDCValue(IntPtr.Zero,
                ref activePolicyGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_HIBERNATEIDLE,
                ref type,
                ref value,
                ref valueSize);

            //if (errorRes == 0)
            //{
            //    MessageBox.Show($"DC Hibernate after {value} seconds.");
            //}

            return value;
        }

        #endregion


        #region 2. 设置进入睡眠/休眠的时间
        /// <summary>
        /// 设置【接电源】时，多久进入睡眠，单位为秒
        /// seconds = 0 时，表示从不睡眠
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetACSleepTime(int seconds)
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activeSchemeGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);

            uint errorRet = PowerWriteACValueIndex(IntPtr.Zero,
                ref activeSchemeGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_STANDBYIDLE,
                seconds);

            if (errorRet == 0)
            {
                //The Changes to the settings for the active power scheme do not take effect until you call the function.
                PowerSetActiveScheme(IntPtr.Zero, ref activeSchemeGuid);
            }

        }


        /// <summary>
        /// 设置使用【电池】时，多久进入睡眠，单位为秒
        /// seconds = 0 时，表示从不睡眠
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetDCSleepTime(int seconds)
        {
            IntPtr activePolicyGuidPTR = IntPtr.Zero;
            uint i = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

            Guid activeSchemeGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);

            uint errorRet = PowerWriteDCValueIndex(IntPtr.Zero,
                ref activeSchemeGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_STANDBYIDLE,
                seconds);

            if (errorRet == 0)
            {
                PowerSetActiveScheme(IntPtr.Zero, ref activeSchemeGuid);
            }
        }

        /// <summary>
        /// 设置接电源时，多久进入休眠，单位为秒
        /// 0 表示永不休眠
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetACHibernateTime(int seconds)
        {
            // 1. Get the current active power scheme and a GUID that identifies the scheme.
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);
            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            // 2. Set the value for the specified power setting.
            uint errorCode_2 = PowerWriteACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_HIBERNATEIDLE,
                seconds);

            if (errorCode_2 == 0)
                PowerSetActiveScheme(IntPtr.Zero,ref powerSchemeGuid);

        }

        /// <summary>
        /// 设置使用电池时，多久进入休眠，单位为秒
        /// 0 表示永不休眠
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetDCHibernateTime(int seconds)
        {
            // 1. Get the current active power scheme and a GUID that identifies the scheme.
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);
            if (errorCode_1 != 0)
                return;
            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            // 2. Set the value for the specified power setting.
            uint errorCode_2 = PowerWriteDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SLEEP_SUBGROUP,
                ref GUID_HIBERNATEIDLE,
                seconds);

            if (errorCode_2 == 0)
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);

        }

        #endregion


        #region 3. 获取/设置 合盖时的行为
        /// <summary>
        /// 获取【接电源】时，合盖的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <returns></returns>
        public static int GetLidCloseACAction()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_LIDCLOSE_ACTION,
                ref resVal);

            return resVal;

        }

        /// <summary>
        /// 获取【使用电池】时，合盖的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <returns></returns>
        public static int GetLidCloseDCAction()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_LIDCLOSE_ACTION,
                ref resVal);

            return resVal;
        }

        /// <summary>
        /// 设置【接电源】时，合盖的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <param name="index"></param>
        public static void SetLidCloseACAction(int index)
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            uint errorCode_2 = PowerWriteACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_LIDCLOSE_ACTION,
                index);

            if (errorCode_2 == 0)
            {
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
            }
        }

        /// <summary>
        /// 设置【使用电池】时，合盖的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <param name="index"></param>
        public static void SetLidCloseDCAction(int index)
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            uint errorCode_2 = PowerWriteDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_LIDCLOSE_ACTION,
                index);

            if (errorCode_2 == 0)
            {
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
            }
        }
        #endregion


        #region 4. 获取/设置 按电源按钮的行为
        /// <summary>
        /// 获取【接电源】时，按电源按钮的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <returns></returns>
        public static int GetPowerBtnACActoin()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_POWERBUTTON_ACTION,
                ref resVal);

            return resVal;
        }

        /// <summary>
        /// 获取【使用电池】时，按电源按钮的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <returns></returns>
        public static int GetPowerBtnDCAction()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_POWERBUTTON_ACTION,
                ref resVal);

            return resVal;
        }

        /// <summary>
        /// 设置【接电源】时，按电源按钮的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <param name="index"></param>
        public static void SetPowerBtnACAction(int index)
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            uint errorCode_2 = PowerWriteACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_POWERBUTTON_ACTION,
                index);

            if (errorCode_2 == 0)
            {
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
            }
        }

        /// <summary>
        /// 设置【使用电池】时，按电源按钮的行为
        /// 0: DoNothing, 1: Sleep, 2: Hibernate, 3: ShutDown
        /// </summary>
        /// <param name="index"></param>
        public static void SetPowerBtnDCAction(int index)
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            uint errorCode_2 = PowerWriteDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_SYSTEM_BUTTON_SUBGROUP,
                ref GUID_POWERBUTTON_ACTION,
                index);

            if (errorCode_2 == 0)
            {
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
            }
        }
        #endregion


        #region 6. 获取/设置 显示器关闭的时间
        /// <summary>
        /// 获取接电源时，关闭显示器的时间，单位为秒
        /// 0 表示永不关闭显示器
        /// </summary>
        public static int GetScreenDisplayACTimeOut()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_VIDEO_SUBGROUP,
                ref GUID_VIDEOIDLE,
                ref resVal);

            return resVal;
        }

        /// <summary>
        /// 获取使用电池时，关闭显示器的时间，单位为秒
        /// 0 表示永不关闭显示器
        /// </summary>
        /// <returns></returns>
        public static int GetScreenDisplayDCTimeOut()
        {
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);

            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);
            int resVal = 0;

            uint errorCode_2 = PowerReadDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_VIDEO_SUBGROUP,
                ref GUID_VIDEOIDLE,
                ref resVal);

            return resVal;
        }

        /// <summary>
        /// 设置接电源时，显示器关闭的时间，单位秒
        /// 0 表示永不关闭
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetScreenOffACTime(int seconds)
        {
            // 1. Get the current active power scheme and a GUID that identifies the scheme.
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);
            if (errorCode_1 != 0)
                return;
            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            // 2. Set the value for the specified power setting.
            uint errorCode_2 = PowerWriteACValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_VIDEO_SUBGROUP,
                ref GUID_VIDEOIDLE,
                seconds);

            if (errorCode_2 == 0)
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
        }

        /// <summary>
        /// 设置使用电池时，显示器关闭的时间，单位秒，
        /// 0表示永不关闭
        /// </summary>
        /// <param name="seconds"></param>
        public static void SetScreenOffDCTime(int seconds)
        {
            // 1. Get the current active power scheme and a GUID that identifies the scheme.
            IntPtr powerSchemeGuidPTR = IntPtr.Zero;
            uint errorCode_1 = PowerGetActiveScheme(IntPtr.Zero, ref powerSchemeGuidPTR);
            if (errorCode_1 != 0)
                return;
            Guid powerSchemeGuid = Marshal.PtrToStructure<Guid>(powerSchemeGuidPTR);

            // 2. Set the value for the specified power setting.
            uint errorCode_2 = PowerWriteDCValueIndex(IntPtr.Zero,
                ref powerSchemeGuid,
                ref GUID_VIDEO_SUBGROUP,
                ref GUID_VIDEOIDLE,
                seconds);

            if (errorCode_2 == 0)
                PowerSetActiveScheme(IntPtr.Zero, ref powerSchemeGuid);
        }


        #endregion



    }
}
