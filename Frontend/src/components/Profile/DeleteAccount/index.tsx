import React, { useState } from "react";
import { Card, CardContent, Button, CardActions, DialogTitle, Dialog, DialogActions } from "@material-ui/core";
import { IUser } from "../../../api/types";
import { gql, useMutation } from "@apollo/client";
import { Alert } from "../../Alert";
import { Redirect } from "react-router-dom";

interface IDeleteAccountProps {
    user: IUser;
    setUser(user: IUser | null): void;
}

export default (props: IDeleteAccountProps) => {
    const [ hasDeletePopup, setHasDeletePopup] = useState(false);
    const [ errorMessage, setErrorMessage] = useState<string | null>(null);
    const [ done, setDone] = useState(false);
    const [ deleteUser ] =
            useMutation(
                DELETE_USER,
                {
                    variables: { userId: props.user.id },
                    onCompleted: (data) =>
                    {
                        if (data?.applicationUser?.deleteUser?.succeeded) {
                            props.setUser(null);
                            setDone(true);
                        } else {
                            let error = data.applicationUser.deleteUser.errors.map((e: any) => e.description).join(", ");
                            setErrorMessage(error);
                        }
                    },
                    onError: (data) => {
                        setErrorMessage(data.message);
                    }
                });

    return <React.Fragment>
        { errorMessage ? <Alert type="error" text={errorMessage} /> : null }
        { done ? <Redirect to="/" /> : null }
        <Dialog onClose={() => setHasDeletePopup(false)} open={hasDeletePopup}>
            <DialogTitle>
                Are you sure you want to delete your account?
            </DialogTitle>
            <DialogActions>
                <Button onClick={() => deleteUser()}>Remove my account</Button>
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
    </React.Fragment>;
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