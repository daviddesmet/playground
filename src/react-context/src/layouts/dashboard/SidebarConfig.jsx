import React from "react";
import { Icon } from "@iconify/react";
import pieChart2Fill from "@iconify/icons-eva/pie-chart-2-fill";
import peopleFill from "@iconify/icons-eva/people-fill";
import checkmarkSquare2Fill from "@iconify/icons-eva/checkmark-square-2-fill";
import alertCircleFill from "@iconify/icons-eva/alert-circle-fill";

// import alertTriangleFill from "@iconify/icons-eva/alert-triangle-fill";

const getIcon = (name) => <Icon icon={name} width={22} height={22} />;

const sidebarConfig = [
  {
    title: "dashboard",
    path: "/dashboard/app",
    icon: getIcon(pieChart2Fill),
  },
  {
    title: "users",
    path: "/dashboard/users",
    icon: getIcon(peopleFill),
  },
  {
    title: "tasks",
    path: "/dashboard/tasks",
    icon: getIcon(checkmarkSquare2Fill),
  },
  {
    title: "about",
    path: "/about",
    icon: getIcon(alertCircleFill),
  },
  // {
  //   title: "Not found",
  //   path: "/404",
  //   icon: getIcon(alertTriangleFill)
  // }
];

export default sidebarConfig;
