import React, { useState } from "react";
import { Card, CardContent, Button, CardActions, DialogTitle, Dialog, DialogActions } from "@material-ui/core";
import { IUser } from "../../../api/types";
import { gql, ApolloCache, ApolloConsumer, ApolloClient } from "@apollo/client";
import { useQuery } from "@apollo/client";
import { Alert } from "../../Alert";

interface IDeleteAccountProps {
    user: IUser;
    setUser(user: IUser | null): void;
}

interface IDeleteAccountState {
    hasDeletePopup: boolean;
    busy: boolean;
    errorMessage: string | null;
}

export default (props: IDeleteAccountProps) => {
    const [hasDeletePopup, setHasDeletePopup] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [isDeleting, setIsDeleting] = useState(false);

    const deleteAccount = () => {
        /*
        if (busy) {
            setBusy(true);
            useQuery(
                DELETE_USER,
                {
                    variables: { userId: props.user.id },
                    onCompleted: (data) => deleteDone(data)
                });
        }
        */
    }

    const deleteDone = (data: any) => {
        /*
        closeDialog();
        setBusy(false);
        if (data.errors || !data?.applicationUser?.deleteUser?.succeeded) {
            let errors: string[] =
                data.errors ?
                     data.errors :
                     data.applicationUser.deleteUser.errors.map((e: any) => e.description);
            setErrorMessage(errors.join(", "));
        } else {
            props.setUser(null);
            window.location.href = '/';
        }
        */
    }

    const openDialog = () => {
        /*
        setHasDeletePopup(true);
        */
    }

    const closeDialog = () => {
        /*
        setHasDeletePopup(false);
        */
    }

    return <Card>
        { errorMessage ? <Alert type="error" text={errorMessage} /> : null }
        <Dialog onClose={() => closeDialog()} open={hasDeletePopup}>
            <DialogTitle>
                Are you sure you want to delete your account?
            </DialogTitle>
            <DialogActions>
                <Button onClick={() => deleteAccount()}>Remove my account</Button>
                <Button onClick={() => closeDialog()}>Don't remove my account</Button>
            </DialogActions>
        </Dialog>
        <CardContent>
            <h3>Delete Account</h3>
            <p> We're sorry if you want to leave... however, if you really want to, you can delete your account here.</p>
        </CardContent>
        <CardActions>
            <Button onClick={() => openDialog()}>Remove my account</Button>
        </CardActions>
    </Card>;
};

const DELETE_USER = gql`
    mutation DeleteUser($userId: ID!)
    {  
        applicationUser {
            deleteUser(id: $userId) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }`;