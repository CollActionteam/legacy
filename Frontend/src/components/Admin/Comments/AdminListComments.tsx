import React, { useState } from "react";
import { Paper, TableContainer, Table, TableHead, TableCell, TableRow, TableBody, TablePagination, Dialog, DialogTitle, DialogContent, DialogActions, MenuItem, Select, InputLabel } from "@material-ui/core";
import { gql, useQuery, useMutation } from "@apollo/client";
import { ICrowdactionComment } from "../../../api/types";
import { Alert } from "../../Alert/Alert";
import Loader from "../../Loader/Loader";
import Formatter from "../../../formatter";
import { Button } from "../../Button/Button";
import { useSettings } from "../../../providers/SettingsProvider";

export default () => {
    const { crowdactionCommentStatusses } = useSettings();
    const [status, setStatus] = useState("NONE");
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [approveDialogOpen, setApproveDialogOpen] = useState(false);
    const [toMutate, setToMutate] = useState<ICrowdactionComment | null>(null);
    const [info, setInfo] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const {data, loading, error: loadingError} = useQuery(
        GET_COMMENTS,
        {
            fetchPolicy: "cache-and-network", // To ensure it updates after deleting/editting
            variables: {
                skip: rowsPerPage * page,
                take: rowsPerPage,
                orderBy: "id",
                status: status === "NONE" ? null : status
            }
        }
    );
    const [ approveComment ] = useMutation(
        APPROVE_COMMENT,
        {
            variables: {
                id: toMutate?.id
            },
            onCompleted: (data) => {
                setApproveDialogOpen(false);
                setError(null);
                setInfo("Successfully approved comment");
            },
            onError: (data) => {
                setApproveDialogOpen(false);
                setError(data.message);
                console.error(data.message);
                setInfo(null);
            }
        }
    );
    const [ deleteComment ] = useMutation(
        DELETE_COMMENT,
        {
            variables: {
                id: toMutate?.id
            },
            onCompleted: (data) => {
                setDeleteDialogOpen(false);
                setError(null);
                setInfo("Successfully deleted comment");
            },
            onError: (data) => {
                setDeleteDialogOpen(false);
                setError(data.message);
                console.error(data.message);
                setInfo(null);
            },
            awaitRefetchQueries: true,
            refetchQueries: [{
                query: GET_COMMENTS,
                variables: {
                    skip: rowsPerPage * page,
                    take: rowsPerPage,
                    orderBy: "id"
                }
            }]
        }
    );
    const commentCount = data?.crowdactionCommentCount ?? 0;

    return <>
        { loading ? <Loader /> : null }
        <Alert type="info" text={info} />
        <Alert type="error" text={error} />
        <Alert type="error" text={loadingError?.message} />
        <Dialog open={approveDialogOpen} onClose={() => setApproveDialogOpen(false)}>
            <DialogTitle>Approve comment?</DialogTitle>
            <DialogContent>
                Are you sure you wish to approve this comment?
            </DialogContent>
            <DialogActions>
                <Button onClick={() => approveComment()}>Yes</Button>
                <Button onClick={() => setApproveDialogOpen(false)}>Cancel</Button>
            </DialogActions>
        </Dialog>
        <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
            <DialogTitle>Delete comment?</DialogTitle>
            <DialogContent>
                Are you sure you wish to delete this comment?
            </DialogContent>
            <DialogActions>
                <Button onClick={() => deleteComment()}>Yes</Button>
                <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
            </DialogActions>
        </Dialog>
        <InputLabel shrink id="status">Status</InputLabel>
        <Select name="status" labelId="status" value={status} onChange={(ev) => setStatus(ev.target.value as string)}>
            <MenuItem key="" value="NONE">NONE</MenuItem>
            { crowdactionCommentStatusses.map(c => <MenuItem key={c} value={c}>{c}</MenuItem>) }
        </Select>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell><h4>User</h4></TableCell>
                        <TableCell align="left"><h4>Crowdaction</h4></TableCell>
                        <TableCell align="left"><h4>Date</h4></TableCell>
                        <TableCell align="left"><h4>Status</h4></TableCell>
                        <TableCell align="left"><h4>Comment</h4></TableCell>
                        <TableCell align="center"><h4>Approve</h4></TableCell>
                        <TableCell align="center"><h4>Delete</h4></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    { data?.crowdactionComments.map((u: ICrowdactionComment) => (
                        <TableRow key={u.id}>
                            <TableCell component="th" scope="row">{ u.user?.fullName ?? u.anonymousCommentUser }</TableCell>
                            <TableCell align="left">{ u.crowdaction?.name }</TableCell>
                            <TableCell align="left">{ Formatter.date(new Date(u.commentedAt)) }</TableCell>
                            <TableCell align="left">{ u.status }</TableCell>
                            <TableCell align="left">{ u.comment }</TableCell>
                            <TableCell align="center">{ u.status !== 'APPROVED' && <Button onClick={() => { setApproveDialogOpen(true); setToMutate(u)}}>Approve</Button> }</TableCell>
                            <TableCell align="center">{ u.status !== 'DELETED' && <Button onClick={() => { setDeleteDialogOpen(true); setToMutate(u); }}>Delete</Button> }</TableCell>
                        </TableRow>))
                    }
                    <TableRow>
                        <TablePagination count={commentCount} page={page} rowsPerPageOptions={[5, 10, 25, 50]} rowsPerPage={rowsPerPage} onChangePage={(_ev, newPage) => setPage(newPage)} onChangeRowsPerPage={(ev) => { setPage(0); setRowsPerPage(parseInt((ev.target.value))) } } />
                    </TableRow>
                </TableBody>
            </Table>
        </TableContainer>
    </>;
};

const GET_COMMENTS = gql`
    query GetCommentData($skip: Int!, $take: Int!, $orderBy: String!, $status: CrowdactionCommentStatus) {
        crowdactionComments(orderBy: [{ path: $orderBy, descending: true}], skip: $skip, take: $take, status: $status) {
          id
          comment
          commentedAt
          status
          anonymousCommentUser
          crowdaction {
            name
          }
          user {
            id
            fullName
          }
        }
        crowdactionCommentCount(status: $status)
    }
`;

const DELETE_COMMENT = gql`
    mutation DeleteComment($id: ID!) {
        crowdaction {
            deleteComment(commentId: $id)
        }
    }
`;

const APPROVE_COMMENT = gql`
    mutation Approve($id: ID!) {
        crowdaction {
            approveComment(commentId: $id) {
              id
              comment
              commentedAt
              status
              anonymousCommentUser
              user {
                id
                fullName
              }
            }
        }
    }
`;