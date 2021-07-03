import USERLIST from "../../_mocks_/user";
import { isEmptyUuid, newUuid } from "../../utils";

export const getUsers = () => {
  return USERLIST;
};

export const getUserById = (state, id) => {
  return state.userList.find((user) => user.id === id);
};

export const saveUser = (setState, user) => {
  setState((currentState) => {
    if (isEmptyUuid(user.id)) {
      user.id = newUuid();
      const newUserList = currentState.userList;
      newUserList.push(user);
      return { ...currentState, userList: newUserList };
    } else {
      const idx = currentState.userList.findIndex((u) => u.id === user.id);
      const newUserList = currentState.userList;
      newUserList[idx] = user;
      return { ...currentState, userList: newUserList };
    }
  });
};

export const setSelectedUsers = (setState, userIds) => {
  setState((currentState) => ({ ...currentState, selectedUsers: userIds }));
};

export const deleteUsers = (setState) => {
  setState((currentState) => {
    const result = currentState.userList.filter(
      (user) => !currentState.selectedUsers.includes(user.id)
    );

    return { ...currentState, userList: result, selectedUsers: [] };
  });
};

export const deleteUserById = (setState, id) => {
  setState((currentState) => {
    const result = currentState.userList.filter((user) => user.id !== id);

    return { ...currentState, userList: result };
  });
};
