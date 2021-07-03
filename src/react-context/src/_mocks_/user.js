import faker from "faker";
import { sample } from "lodash";
import ACCOUNT from "./account";

const users = [...Array(24)].map((_, index) => ({
  id: faker.datatype.uuid(),
  // avatarUrl: faker.internet.avatar(),
  avatarUrl: faker.image.avatar(),
  name: faker.name.findName(),
  email: faker.internet.email(),
  project: faker.company.companyName(),
  isVerified: faker.datatype.boolean(),
  status: sample(["active", "lazy"]),
  role: sample([
    "Team Lead",
    "Product Owner",
    "UI Designer",
    "UX Designer",
    "UI/UX Designer",
    "Project Manager",
    "Backend Developer",
    "Full Stack Designer",
    "Front End Developer",
    "Full Stack Developer"
  ])
}));

users.push({
  id: ACCOUNT.id,
  avatarUrl: ACCOUNT.photoURL,
  name: ACCOUNT.displayName,
  email: ACCOUNT.email,
  project: ACCOUNT.project,
  isVerified: true,
  status: "badass",
  role: ACCOUNT.role
});

export default users;
