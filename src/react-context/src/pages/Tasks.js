import { filter } from "lodash";
import { Icon } from "@iconify/react";
import { useState } from "react";
import plusFill from "@iconify/icons-eva/plus-fill";
import { Link as RouterLink } from "react-router-dom";
import { useTaskContext } from "../contexts/task";

import {
  Breadcrumbs,
  Card,
  Table,
  Stack,
  Avatar,
  Button,
  Checkbox,
  Link,
  TableRow,
  TableBody,
  TableCell,
  Container,
  Typography,
  TableContainer,
  TablePagination
} from "@material-ui/core";
import Page from "../components/Page";
import Label from "../components/Label";
import Scrollbar from "../components/Scrollbar";
import SearchNotFound from "../components/SearchNotFound";
import {
  TableListHead,
  TableListToolbar,
  TableMoreMenu
} from "../components/_dashboard/table";

const TABLE_HEAD = [
  { id: "name", label: "Task", alignRight: false },
  { id: "project", label: "Project", alignRight: false },
  { id: "user", label: "User", alignRight: false },
  { id: "completed", label: "Completed", alignRight: false },
  { id: "" }
];

function descendingComparator(a, b, orderBy) {
  if (b[orderBy] < a[orderBy]) {
    return -1;
  }
  if (b[orderBy] > a[orderBy]) {
    return 1;
  }
  return 0;
}

function getComparator(order, orderBy) {
  return order === "desc"
    ? (a, b) => descendingComparator(a, b, orderBy)
    : (a, b) => -descendingComparator(a, b, orderBy);
}

function applySortFilter(array, comparator, query) {
  const stabilizedThis = array.map((el, index) => [el, index]);
  stabilizedThis.sort((a, b) => {
    const order = comparator(a[0], b[0]);
    if (order !== 0) return order;
    return a[1] - b[1];
  });
  if (query) {
    return filter(
      array,
      (_task) => _task.name.toLowerCase().indexOf(query.toLowerCase()) !== -1
    );
  }
  return stabilizedThis.map((el) => el[0]);
}

export default function Users() {
  const [page, setPage] = useState(0);
  const [order, setOrder] = useState("asc");
  const [orderBy, setOrderBy] = useState("name");
  const [filterName, setFilterName] = useState("");
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const { state, actions } = useTaskContext();
  const selected = state.selectedTasks;

  const handleRequestSort = (event, property) => {
    const isAsc = orderBy === property && order === "asc";
    setOrder(isAsc ? "desc" : "asc");
    setOrderBy(property);
  };

  const handleSelectAllClick = (event) => {
    if (event.target.checked) {
      const newSelecteds = state.taskList.map((n) => n.id);
      actions.setSelectedTasks(newSelecteds);
      return;
    }
    actions.setSelectedTasks([]);
  };

  const handleClick = (event, id) => {
    const selectedIndex = selected.indexOf(id);
    let newSelected = [];
    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, id);
    } else if (selectedIndex === 0) {
      newSelected = newSelected.concat(selected.slice(1));
    } else if (selectedIndex === selected.length - 1) {
      newSelected = newSelected.concat(selected.slice(0, -1));
    } else if (selectedIndex > 0) {
      newSelected = newSelected.concat(
        selected.slice(0, selectedIndex),
        selected.slice(selectedIndex + 1)
      );
    }
    actions.setSelectedTasks(newSelected);
  };

  const handleDeleteSelected = (event) => {
    actions.deleteSelectedTasks();
  };

  const handleDeleteOne = (id) => {
    actions.deleteTaskById(id);
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleFilterByName = (event) => {
    setFilterName(event.target.value);
  };

  const emptyRows =
    page > 0
      ? Math.max(0, (1 + page) * rowsPerPage - state.taskList.length)
      : 0;

  const filteredTasks = applySortFilter(
    state.taskList,
    getComparator(order, orderBy),
    filterName
  );

  const isUserNotFound = filteredTasks.length === 0;

  return (
    <Page title="Tasks | Definity First University">
      <Container>
        <Stack
          direction="row"
          alignItems="center"
          justifyContent="space-between"
          mb={5}
        >
          <Stack direction="column">
            <Typography variant="h4" gutterBottom>
              Tasks
            </Typography>
            <Breadcrumbs aria-label="breadcrumb">
              <Link
                underline="hover"
                color="inherit"
                component={RouterLink}
                to="/"
              >
                Home
              </Link>
              <Typography color="text.primary">Tasks</Typography>
            </Breadcrumbs>
          </Stack>
          <Button
            variant="contained"
            component={RouterLink}
            to="/dashboard/tasks/00000000-0000-0000-0000-000000000000"
            startIcon={<Icon icon={plusFill} />}
          >
            New Task
          </Button>
        </Stack>

        <Card>
          <TableListToolbar
            placeholder="Search task..."
            numSelected={selected.length}
            filterName={filterName}
            onFilterName={handleFilterByName}
            onDeleteClick={handleDeleteSelected}
          />

          <Scrollbar>
            <TableContainer sx={{ minWidth: 800 }}>
              <Table>
                <TableListHead
                  order={order}
                  orderBy={orderBy}
                  headLabel={TABLE_HEAD}
                  rowCount={state.taskList.length}
                  numSelected={selected.length}
                  onRequestSort={handleRequestSort}
                  onSelectAllClick={handleSelectAllClick}
                />
                <TableBody>
                  {filteredTasks
                    .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                    .map((row) => {
                      const { id, name, user, completed } = row;
                      const isItemSelected = selected.indexOf(id) !== -1;

                      return (
                        <TableRow
                          hover
                          key={id}
                          tabIndex={-1}
                          role="checkbox"
                          selected={isItemSelected}
                          aria-checked={isItemSelected}
                        >
                          <TableCell padding="checkbox">
                            <Checkbox
                              checked={isItemSelected}
                              onChange={(event) => handleClick(event, id)}
                            />
                          </TableCell>
                          <TableCell align="left">{name}</TableCell>
                          <TableCell align="left">{user.project}</TableCell>
                          <TableCell component="th" scope="row">
                            <Stack
                              direction="row"
                              alignItems="center"
                              spacing={2}
                            >
                              <Avatar alt={user.name} src={user.avatarUrl} />
                              <Typography variant="subtitle2" noWrap>
                                {user.name}
                              </Typography>
                            </Stack>
                          </TableCell>
                          <TableCell align="left">
                            <Label
                              variant="ghost"
                              color={completed ? "success" : "error"}
                            >
                              {completed ? "Yes" : "No"}
                            </Label>
                          </TableCell>
                          <TableCell align="right">
                            <TableMoreMenu
                              route={id}
                              onDeleteClick={() => handleDeleteOne(id)}
                            />
                          </TableCell>
                        </TableRow>
                      );
                    })}
                  {emptyRows > 0 && (
                    <TableRow style={{ height: 53 * emptyRows }}>
                      <TableCell colSpan={6} />
                    </TableRow>
                  )}
                </TableBody>
                {isUserNotFound && (
                  <TableBody>
                    <TableRow>
                      <TableCell align="center" colSpan={6} sx={{ py: 3 }}>
                        <SearchNotFound searchQuery={filterName} />
                      </TableCell>
                    </TableRow>
                  </TableBody>
                )}
              </Table>
            </TableContainer>
          </Scrollbar>

          <TablePagination
            rowsPerPageOptions={[5, 10, 25]}
            component="div"
            count={state.taskList.length}
            rowsPerPage={rowsPerPage}
            page={page}
            onPageChange={handleChangePage}
            onRowsPerPageChange={handleChangeRowsPerPage}
          />
        </Card>
      </Container>
    </Page>
  );
}
