import TASKLIST from "../../_mocks_/task";
import { isEmptyUuid, newUuid } from "../../utils";

export const getTasks = () => {
  return TASKLIST;
};

export const getTaskById = (state, id) => {
  return state.taskList.find((task) => task.id === id);
};

export const saveTask = (setState, task) => {
  setState((currentState) => {
    if (isEmptyUuid(task.id)) {
      task.id = newUuid();
      const newTaskList = currentState.taskList;
      newTaskList.push(task);
      return { ...currentState, taskList: newTaskList };
    } else {
      const idx = currentState.taskList.findIndex((t) => t.id === task.id);
      const newTaskList = currentState.taskList;
      newTaskList[idx] = task;
      return { ...currentState, taskList: newTaskList };
    }
  });
};

export const setSelectedTasks = (setState, taskIds) => {
  setState((currentState) => ({ ...currentState, selectedTasks: taskIds }));
};

export const deleteTasks = (setState) => {
  setState((currentState) => {
    const result = currentState.taskList.filter(
      (task) => !currentState.selectedTasks.includes(task.id)
    );

    return { ...currentState, taskList: result, selectedTasks: [] };
  });
};

export const deleteTaskById = (setState, id) => {
  setState((currentState) => {
    const result = currentState.taskList.filter((task) => task.id !== id);

    return { ...currentState, taskList: result };
  });
};
