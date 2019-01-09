import { TimelineRecord } from "@/models/timeline.interface";
import { UPDATE_TIMELINE } from "./actions.type";

export enum Status {
  Init,
  Success,
  Error
}

const getStatusColor = (status: Status): string => {
  switch (status) {
    case Status.Success:
      return "green";
    case Status.Error:
      return "red";
    default:
      // Init
      return "amber";
  }
};

export const log = (dispatch: any, title: string, message: string, status: Status) => {
  const color = getStatusColor(status);
  dispatch(`timeline/${UPDATE_TIMELINE}`, { title, message, color } as TimelineRecord, { root: true });
};
