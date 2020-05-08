import React, { useState } from "react";
import { Paper, TableContainer, Table, TableHead, TableCell, TableRow, TableBody, Button, TablePagination, Dialog, DialogTitle, DialogContent, DialogActions } from "@material-ui/core";
import { gql, useQuery, useMutation } from "@apollo/client";
import { ICrowdaction } from "../../../api/types";
import { useHistory } from "react-router-dom";
import Loader from "../../Loader/Loader";
import { Alert } from "../../Alert/Alert";
import { Fragments } from "../../../api/fragments";
import Formatter from "../../../formatter";

export default () => {
    const history = useHistory();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [toDelete, setToDelete] = useState<ICrowdaction | null>(null);
    const [info, setInfo] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const {data, loading, error: loadingError} = useQuery(
        GET_CROWDACTIONS,
        {
            fetchPolicy: "cache-and-network", // To ensure it updates after deleting/editting
            variables: {
                skip: rowsPerPage * page,
                take: rowsPerPage,
                orderBy: "name"
            }
        }
    );
    const [ deleteCrowdaction ] = useMutation(
        DELETE_CROWDACTION,
        {
            variables: {
                id: toDelete?.id
            },
            onCompleted: (_data) => {
                setDeleteDialogOpen(false);
                setError(null);
            },
            onError: (data) => {
                setDeleteDialogOpen(false);
                setInfo(null);
                setError(data.message);
                console.error(data.message);
            },
            awaitRefetchQueries: true,
            refetchQueries: [{ 
                query: GET_CROWDACTIONS,
                variables: {
                    skip: rowsPerPage * page,
                    take: rowsPerPage,
                    orderBy: "name"
                }
            }]
        }
    );
    const crowdactionCount = data?.crowdactionCount ?? 0;

    return <>
        { loading ? <Loader /> : null }
        <Alert type="info" text={info} />
        <Alert type="error" text={error} />
        <Alert type="error" text={loadingError?.message} />
        <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
            <DialogTitle>Delete crowdaction { toDelete?.name }?</DialogTitle>
            <DialogContent>
                Are you sure you wish to delete "{ toDelete?.name }"?
            </DialogContent>
            <DialogActions>
                <Button onClick={() => deleteCrowdaction()}>Yes</Button>
                <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
            </DialogActions>
        </Dialog>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell><h4>Name</h4></TableCell>
                        <TableCell align="right"><h4>Status</h4></TableCell>
                        <TableCell align="right"><h4>Start</h4></TableCell>
                        <TableCell align="right"><h4>End</h4></TableCell>
                        <TableCell align="right"><h4>Active</h4></TableCell>
                        <TableCell align="right"><h4>Edit</h4></TableCell>
                        <TableCell align="right"><h4>Delete</h4></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    { data?.crowdactions.map((p: ICrowdaction) => (
                        <TableRow key={p.id}>
                            <TableCell component="th" scope="row">{ p.name }</TableCell>
                            <TableCell align="right">{ p.status }</TableCell>
                            <TableCell align="right">{ Formatter.date(new Date(p.start)) }</TableCell>
                            <TableCell align="right">{ Formatter.date(new Date(p.end)) } { Formatter.time(new Date(p.end)) }</TableCell>
                            <TableCell align="right">{ p.isActive ? "Yes" : "No" }</TableCell>
                            <TableCell align="right"><Button onClick={() => history.push(`/admin/crowdactions/edit/${p.id}`)}>Edit</Button></TableCell>
                            <TableCell align="right"><Button onClick={() => { setDeleteDialogOpen(true); setToDelete(p); }}>Delete</Button></TableCell>
                        </TableRow>))
                    }
                    <TableRow>
                        <TablePagination count={crowdactionCount} page={page} rowsPerPageOptions={[5, 10, 25, 50]} rowsPerPage={rowsPerPage} onChangePage={(_ev, newPage) => setPage(newPage)} onChangeRowsPerPage={(ev) => { setPage(0); setRowsPerPage(parseInt((ev.target.value))) }} />
                    </TableRow>
                </TableBody>
            </Table>
        </TableContainer>
    </>;
};

const GET_CROWDACTIONS = gql`
    query GetCrowdactionData($skip: Int!, $take: Int!, $orderBy: String!) {
        crowdactions(orderBy: [{ path: $orderBy, descending: false}], skip: $skip, take: $take) {
            ${Fragments.crowdactionDetail}
        }
        crowdactionCount
    }
`;

const DELETE_CROWDACTION = gql`
    mutation DeleteCrowdaction($id: ID!) {
        crowdaction {
            deleteCrowdaction(id: $id)
        }
    }
`;