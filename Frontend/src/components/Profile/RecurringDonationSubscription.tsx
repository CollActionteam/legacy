import { IDonationSubscription, IUser } from "../../api/types";
import React, { useState } from "react";
import { ListItemAvatar, Avatar, ListItemText, ListItemSecondaryAction, IconButton, ListItem, Button, Dialog, DialogTitle, DialogActions } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useMutation } from "@apollo/client/react/hooks/useMutation";
import { gql } from "@apollo/client";
import { Alert } from "../Alert/Alert";
import { GET_USER } from "../../providers/user";

interface IRecurringDonationItemProps {
    user: IUser;
    subscription: IDonationSubscription;
}

export default ({ user, subscription }: IRecurringDonationItemProps) => {
    const [ stopSubscription ] = useMutation(STOP_SUBSCRIPTION,
        {
            variables: {
                subscriptionId: subscription.id
            },
            onCompleted: (_) => {
                user.donationSubscriptions = user.donationSubscriptions.filter(sub => sub.id !== subscription.id);
            },
            onError: (data) => {
                setErrorMessage(data.message);
            },
            refetchQueries: [{
                query: GET_USER
            }]
        });
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ hasStopPopup, setHasStopPopup ] = useState(false);

    return <React.Fragment>
        <Alert type="error" text={errorMessage} />
        <Dialog onClose={() => setHasStopPopup(false)} open={hasStopPopup}>
            <DialogTitle>
                Are you sure you want to delete your account?
            </DialogTitle>
            <DialogActions>
                <Button onClick={() => stopSubscription()}>Stop this donation subscription</Button>
                <Button onClick={() => setHasStopPopup(false)}>Don't stop this donation subscription</Button>
            </DialogActions>
        </Dialog>
        <ListItem>
            <ListItemAvatar>
            <Avatar>
                <FontAwesomeIcon icon="euro-sign" />
            </Avatar>
            </ListItemAvatar>
            <ListItemText primary={`Recurring donation started at ${subscription.startDate}`} />
            <ListItemSecondaryAction>
            <IconButton onClick={() => setHasStopPopup(true)}>
                <FontAwesomeIcon icon="trash" />
            </IconButton>
            </ListItemSecondaryAction>
        </ListItem>
    </React.Fragment>;
};

const STOP_SUBSCRIPTION = gql`
    mutation StopSubscription($subscriptionId: ID!)
    {  
        donation {
            cancelSubscription(subscriptionId: $subscriptionId)
        }
    }
`;