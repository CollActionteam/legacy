import React, { useState } from "react";
import { Card, CardContent, CardActions, DialogTitle, Dialog, DialogActions } from "@material-ui/core";
import { IUser } from "../../api/types";
import { gql, useMutation } from "@apollo/client";
import { Alert } from "../Alert/Alert";
import { GET_USER } from "../../providers/UserProvider";
import { Button } from "../Button/Button";
import { useAnalytics } from "../../providers/AnalyticsProvider";

interface IDeleteAccountProps {
    user: IUser;
}

const DeleteAccount = ({ user }: IDeleteAccountProps) => {
    const [ hasDeletePopup, setHasDeletePopup] = useState(false);
    const [ errorMessage, setErrorMessage] = useState<string | null>(null);
    const { sendUserEvent } = useAnalytics();
    const [ deleteUser ] =
            useMutation(
                DELETE_USER,
                {
                    variables: { userId: user.id },
                    onCompleted: (data) => {
                        if (data.user.deleteUser.succeeded) {
                            // After the account is deleted, navigate to root with hard refresh so that the cache is cleared and the user is logged out
                            window.location.href = '/';
                        } else {
                            let error = data.user.deleteUser.errors.map((e: any) => e.description).join(", ");
                            console.error(error);
                            setErrorMessage(error);
                        }
                    },
                    onError: (data) => {
                        console.error(data.message);
                        setErrorMessage(data.message);
                    },
                    refetchQueries: [{
                        query: GET_USER
                    }]
                });

    return <>
        <Alert type="error" text={errorMessage} />
        <Dialog onClose={() => setHasDeletePopup(false)} open={hasDeletePopup}>
            <DialogTitle>
                Are you sure you want to delete your account?
            </DialogTitle>
            <DialogActions>
                <Button onClick={() => { deleteUser(); sendUserEvent(false, 'user', 'delete_account', '', null); }}>Remove my account</Button>
                <Button onClick={() => setHasDeletePopup(false)}>Don't remove my account</Button>
            </DialogActions>
        </Dialog>
        <Card>
            <CardContent>
                <h3>Delete Account</h3>
                <p> We're sorry if you want to leave... however, if you really want to, you can delete your account here.</p>
            </CardContent>
            <CardActions>
                <Button onClick={() => setHasDeletePopup(true)}>Remove my account</Button>
            </CardActions>
        </Card>
    </>;
};

const DELETE_USER = gql`
    mutation DeleteUser($userId: ID!)
    {  
        user {
            deleteUser(id: $userId) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }`;

export default DeleteAccount;