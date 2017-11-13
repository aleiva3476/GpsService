
namespace GPSService.Meiligao
{
    public enum CommandTypes : int
    {
        NONE = 0x0000,
        LOGIN = 0x5000, // sent by tracker
        CONFIRM_LOGIN = 0x4000,
        GET_SN_IMEI = 0x9001,
        REQUEST_REPORT = 0x4101,
        RESET_CONFIGURATION = 0x4110,
        REBOOT_GPS = 0x4902,
        SET_EXTENDED_SETTINGS = 0x4108,
        SET_HEARTBEAT_INTERVAL = 0x5199,
        CLEAR_MILEAGE = 0x4351,
        SET_POWER_DOWN_TIMEOUT = 0x4126,

        GET_MEMORY_REPORT = 0x9016,
        SET_MEMORY_REPORT_INTERVAL = 0x4131,
        CLEAR_MEMORY_REPORTS = 0x5503,

        GET_AUTHORIZED_PHONE = 0x9003,
        SET_AUTHORIZED_PHONE = 0x4103,

        GET_REPORT_TIME_INTERVAL = 0x9002,
        SET_REPORT_TIME_INTERVAL = 0x4102,
        SET_REPORT_TIME_INTERVAL_RESULT = 0x5100,
        SET_REPORT_DISTANCE_INTERVAL = 0x4303,

        SET_ALARM_SPEEDING = 0x4105,
        SET_ALARM_MOVEMENT = 0x4106,
        SET_ALARM_GEOFENCE = 0x4302,

        REPORT = 0x9955, // sent by tracker
        ALARM = 0x9999, // sent by tracker
    }

    public enum MessageTypes : int
    {
        REPORT_POWER_ON = 0x14,
        REPORT_BY_TIME = 0x9955,
        REPORT_BY_DISTANCE = 0x63,
        REPORT_BLIND_AREA_START = 0x15,
        REPORT_BLIND_AREA_END = 0x16,
        REPORT_DIRECTION_CHANGE = 0x52,

        ALARM_SOS_PRESSED = 0x01,
        ALARM_SOS_RELEASED = 0x31,
        ALARM_LOW_BATTERY = 0x10,
        ALARM_SPEEDING = 0x11,
        ALARM_MOVEMENT = 0x12, // movement & geo-fence
    }
}
