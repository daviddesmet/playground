import { TimelineRecord } from "@/models/timeline.interface";

export interface TimelineState {
  status?: string;
  timeline: TimelineRecord[];
}
