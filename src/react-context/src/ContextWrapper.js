import { UserProvider } from "./contexts";
import { TaskProvider } from "./contexts";

function ContextWrapper({ children }) {
  return (
    <UserProvider>
      <TaskProvider>{children}</TaskProvider>
    </UserProvider>
  );
}

export default ContextWrapper;
