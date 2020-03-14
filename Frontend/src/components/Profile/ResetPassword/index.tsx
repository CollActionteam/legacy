import React, { useState } from "react";
import { Card, CardContent, CardActions, Button, TextField, FormGroup } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import { Alert } from "../../Alert";

export default () => {
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmNewPassword, setConfirmNewPassword] = useState("");
    const [success, setSuccess] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [ changePassword ] = useMutation(
        CHANGE_PASSWORD,
        {
            variables: {
                currentPassword: currentPassword,
                newPassword: newPassword
            },
            onCompleted: (data) => {
                if (data.applicationUser.changePassword.succeeded) {
                    setSuccess(true);
                } else {
                    let error = data.applicationUser.changePassword.errors.map((e: any) => e.description).join(", ");
                    setSuccess(false);
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setSuccess(false);
                setErrorMessage(data.message);
            }
        }
    );

    const isValid = confirmNewPassword === newPassword && currentPassword.length > 0 && newPassword.length > 0;

    return <React.Fragment>
        { errorMessage ? <Alert type="error" text={errorMessage} /> : null }
        { success ? <Alert type="success" text="Your password has been changed" /> : null }
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
        applicationUser {
            changePassword(currentPassword: $currentPassword, newPassword: $newPassword) {
            succeeded
            errors {
                code
                description
            }
            }
        }
    }`;