import React from "react";
import { Card, CardContent, CardActions, Button } from "@material-ui/core";
import { IUser } from "../../../api/types";

interface IResetPasswordProps {
    user: IUser;
}

export default (props: IResetPasswordProps) => {
    return <Card>
        <CardContent>
            <h3>Password</h3>
            <p>Changing your password regularly and using different passwords for different sites helps keep your digital identity safe. Tip: use a password manager!</p>
        </CardContent>
        <CardActions>
            <Button>Change your password</Button>
        </CardActions>
    </Card>;
};