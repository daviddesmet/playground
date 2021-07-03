import faker from "faker";

export const isEmptyUuid = (id) => {
  return id === "00000000-0000-0000-0000-000000000000";
};

export const newUuid = () => {
  return faker.datatype.uuid();
};
