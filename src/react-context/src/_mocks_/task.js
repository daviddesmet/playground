import faker from "faker";
import { sentenceCase } from "change-case";
import { sample } from "lodash";
import ACCOUNT from "./account";
import USERLIST from "./user";

const tasks = [...Array(24)].map((_, index) => ({
  id: faker.datatype.uuid(),
  name: sentenceCase(faker.git.commitMessage()),
  user: sample(USERLIST),
  completed: faker.datatype.boolean()
}));

const me = USERLIST.find((user) => user.id === ACCOUNT.id);

[
  "Stakeholder Meeting",
  "Scoping & Estimations",
  "Sprint Showcase",
  "Optimize static images",
  "Integrate unit tests in CI pipeline"
].forEach((task) => {
  tasks.push({
    id: faker.datatype.uuid(),
    name: task,
    user: me,
    completed: faker.datatype.boolean()
  });
});

export default tasks;
