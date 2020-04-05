import React, { useState } from "react";
import { Paper, TableContainer, Table, TableHead, TableCell, TableRow, TableBody, Button, TablePagination, Checkbox, Dialog, DialogTitle, DialogContent, DialogActions } from "@material-ui/core";
import { gql, useQuery, useMutation } from "@apollo/client";
import { IUser } from "../../../api/types";
import { useHistory } from "react-router-dom";

import { Alert } from "../../Alert/Alert";
import Loader from "../../Loader/Loader";

export default () => {
    const history = useHistory();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [toDelete, setToDelete] = useState<IUser | null>(null);
    const [info, setInfo] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const {data, loading} = useQuery(
        GET_USERS,
        {
            fetchPolicy: "cache-and-network", // To ensure it updates after deleting/editting
            variables: {
                skip: rowsPerPage * page,
                take: rowsPerPage,
                orderBy: "lastName"
            }
        }
    );
    const [ deleteUser ] = useMutation(
        DELETE_USER,
        {
            variables: {
                id: toDelete?.id
            },
            onCompleted: (data) => {
                setDeleteDialogOpen(false);
                if (data.user.deleteUser.succeeded) {
                    setError(null);
                    setInfo("Successfully deleted user");
                } else {
                    setInfo(null);
                    setError("Error deleting user: " + data.user.deleteUser.errors.map((e: any) => e.description).join(", "));
                }
            },
            onError: (data) => {
                setDeleteDialogOpen(false);
                setError(data.message);
                console.error(data.message);
                setInfo(null);
            },
            awaitRefetchQueries: true,
            refetchQueries: [{
                query: GET_USERS,
                variables: {
                    skip: rowsPerPage * page,
                    take: rowsPerPage,
                    orderBy: "lastName"
                }
            }]
        }
    );
    const userCount = data?.userCount ?? 0;

    return <React.Fragment>
        { loading ? <Loader /> : null }
        <Alert type="info" text={info} />
        <Alert type="error" text={error} />
        <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
            <DialogTitle>Delete user { toDelete?.email }?</DialogTitle>
            <DialogContent>
                Are you sure you wish to delete "{ toDelete?.email }"?
            </DialogContent>
            <DialogActions>
                <Button onClick={() => deleteUser()}>Yes</Button>
                <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
            </DialogActions>
        </Dialog>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell><h4>E-Mail</h4></TableCell>
                        <TableCell align="right"><h4>First Name</h4></TableCell>
                        <TableCell align="right"><h4>Last Name</h4></TableCell>
                        <TableCell align="right"><h4>Is Admin</h4></TableCell>
                        <TableCell align="right"><h4>Registration Date</h4></TableCell>
                        <TableCell align="right"><h4>Edit</h4></TableCell>
                        <TableCell align="right"><h4>Delete</h4></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    { data?.users.map((u: IUser) => (
                        <TableRow key={u.id}>
                            <TableCell component="th" scope="row">{ u.email }</TableCell>
                            <TableCell align="right">{ u.firstName }</TableCell>
                            <TableCell align="right">{ u.lastName }</TableCell>
                            <TableCell align="right"><Checkbox readOnly checked={ u.isAdmin } /></TableCell>
                            <TableCell align="right">{ u.registrationDate }</TableCell>
                            <TableCell align="right"><Button onClick={() => history.push(`/admin/users/edit/${u.id}`)}>Edit</Button></TableCell>
                            <TableCell align="right"><Button onClick={() => { setDeleteDialogOpen(true); setToDelete(u); }}>Delete</Button></TableCell>
                        </TableRow>))
                    }
                    <TableRow>
                        <TablePagination count={userCount} page={page} rowsPerPageOptions={[5, 10, 25, 50]} rowsPerPage={rowsPerPage} onChangePage={(_ev, newPage) => setPage(newPage)} onChangeRowsPerPage={(ev) => { setPage(0); setRowsPerPage(parseInt((ev.target.value))) } } />
                    </TableRow>
                </TableBody>
            </Table>
        </TableContainer>
    </React.Fragment>;
};

const GET_USERS = gql`
    query GetUserData($skip: Int!, $take: Int!, $orderBy: String!) {
        users(orderBy: [{ path: $orderBy, descending: false}], skip: $skip, take: $take) {
            id
            email
            isSubscribedNewsletter
            firstName
            lastName
            isAdmin
            fullName
            registrationDate
            representsNumberParticipants
        }
        userCount
    }
`;

const DELETE_USER = gql`
    mutation DeleteUser($id: ID!) {
        user {
            deleteUser(id: $id) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }
`;