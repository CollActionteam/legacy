import React, { useState } from "react";
import { Card, CardContent, CardActions, TextField, FormGroup } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import { Alert } from "../Alert/Alert";
import { Button } from "../Button/Button";

export default () => {
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmNewPassword, setConfirmNewPassword] = useState("");
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [ changePassword ] = useMutation(
        CHANGE_PASSWORD,
        {
            variables: {
                currentPassword: currentPassword,
                newPassword: newPassword
            },
            onCompleted: (data) => {
                if (data.user.changePassword.succeeded) {
                    setSuccessMessage("Your password has been changed");
                } else {
                    let error = data.user.changePassword.errors.map((e: any) => e.description).join(", ");
                    setSuccessMessage(null);
                    setErrorMessage(error);
                    console.error(error);
                }
            },
            onError: (data) => {
                setSuccessMessage(null);
                setErrorMessage(data.message);
            }
        }
    );

    const isValid = confirmNewPassword === newPassword && currentPassword.length > 0 && newPassword.length > 0;

    return <React.Fragment>
        <Alert type="error" text={errorMessage} />
        <Alert type="success" text={successMessage} />
        <Card>
            <CardContent>
                <h3>Password</h3>
                <p>Changing your password regularly and using different passwords for different sites helps keep your digital identity safe. Tip: use a password manager!</p>
            </CardContent>
            <CardActions>
                <FormGroup>
                    <TextField onChange={(action) => setCurrentPassword(action.target.value)} value={currentPassword} required label="Current Password" type="password" />
                    <TextField onChange={(action) => setNewPassword(action.target.value)} value={newPassword} required label="New Password" type="password" />
                    <TextField onChange={(action) => setConfirmNewPassword(action.target.value)} value={confirmNewPassword} required error={newPassword !== confirmNewPassword} label="Confirm New Password" type="password" />
                    <Button onClick={() => changePassword()} disabled={!isValid}>Change your password</Button>
                </FormGroup>
            </CardActions>
        </Card>
    </React.Fragment>;
};

const CHANGE_PASSWORD = gql`
    mutation ChangePassword($currentPassword: String!, $newPassword: String!)
    {  
        user {
            changePassword(currentPassword: $currentPassword, newPassword: $newPassword) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }`;