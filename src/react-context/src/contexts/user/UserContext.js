import { createContext, useContext, useEffect, useState } from "react";
import {
  getUsers,
  getUserById,
  setSelectedUsers,
  saveUser,
  deleteUsers,
  deleteUserById
} from "./actions";

const UserContext = createContext();

function UserProvider({ children }) {
  const [state, setState] = useState({
    userList: [],
    selectedUsers: []
  });

  useEffect(() => {
    (function () {
      const userList = getUsers();
      setState((currentState) => ({ ...currentState, userList }));
    })();
  }, []);

  // context actions
  const handleGetUserById = (userId) => {
    return getUserById(state, userId);
  };

  const handleSaveUser = (user) => {
    saveUser(setState, user);
  };

  const handleSelectedUsers = (userIds) => {
    setSelectedUsers(setState, userIds);
  };

  const handleDeleteSelectedUsers = () => {
    deleteUsers(setState);
  };

  const handleDeleteUserById = (id) => {
    deleteUserById(setState, id);
  };

  return (
    <UserContext.Provider
      value={{
        state,
        actions: {
          getUser: handleGetUserById,
          setSelectedUsers: handleSelectedUsers,
          saveUser: handleSaveUser,
          deleteSelectedUsers: handleDeleteSelectedUsers,
          deleteUserById: handleDeleteUserById
        }
      }}
    >
      {children}
    </UserContext.Provider>
  );
}

export function useUserContext() {
  return useContext(UserContext);
}

export default UserProvider;
