import React from "react";
import { IUser } from "../../../api/types";
import { CardActions, CardContent, Card } from "@material-ui/core";

interface IProjectCreatedProps {
    user: IUser;
}

export default (props: IProjectCreatedProps) => {
    return <Card>
        <CardContent>
            <h3>Projects Created</h3>
        </CardContent>
        <CardActions>
        </CardActions>
    </Card>;
};