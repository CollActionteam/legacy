import React, { useState } from "react";
import { Card, CardContent, Button, CardActions, DialogTitle, Dialog, DialogActions } from "@material-ui/core";
import { IUser } from "../../../api/types";
import { gql, useMutation } from "@apollo/client";
import { Alert } from "../../Alert/Alert";
import { useHistory } from "react-router-dom";
import { GET_USER } from "../../../providers/user";

interface IDeleteAccountProps {
    user: IUser;
}

export default ({ user }: IDeleteAccountProps) => {
    const [ hasDeletePopup, setHasDeletePopup] = useState(false);
    const [ errorMessage, setErrorMessage] = useState<string | null>(null);
    const history = useHistory();
    const [ deleteUser ] =
            useMutation(
                DELETE_USER,
                {
                    variables: { userId: user.id },
                    onCompleted: (data) =>
                    {
                        if (data.user.deleteUser.succeeded) {
                            history.push("/");
                        } else {
                            let error = data.user.deleteUser.errors.map((e: any) => e.description).join(", ");
                            setErrorMessage(error);
                        }
                    },
                    onError: (data) => {
                        setErrorMessage(data.message);
                    },
                    refetchQueries: [{
                        query: GET_USER
                    }]
                });

    return <React.Fragment>
        <Alert type="error" text={errorMessage} />
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