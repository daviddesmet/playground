import { Profile } from "@/models/profile.interface";

export interface UserState {
  profile?: Profile;
  status?: string;
}
