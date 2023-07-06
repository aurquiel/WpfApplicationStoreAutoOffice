using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskClassLibrary;

namespace WinformsClassLibrary
{
    public class Notifications
    {
        private static NotifyIcon notificationIcon = new NotifyIcon();

        public static void ShowNotification(string descpricion, TaskOperators.EnumTaskStatusTask tasKStatus)
        {
            notificationIcon.Icon = SystemIcons.Information;
            notificationIcon.BalloonTipText = descpricion;
            if (tasKStatus == TaskOperators.EnumTaskStatusTask.PENDIENTE)
            {
                notificationIcon.BalloonTipTitle = "Nuevas Tareas";
                notificationIcon.BalloonTipIcon = ToolTipIcon.Info;
                notificationIcon.Visible = true;
                notificationIcon.ShowBalloonTip(500000);
            }
        }
    }
}
