import React, { Component } from 'react';
import buildGraphQLProvider from 'ra-data-graphql';
import { Admin, Resource, Delete, List, Datagrid, Edit, Create, BooleanInput, NumberInput, SimpleForm, DateField, TextField, EditButton, TextInput, BooleanField, DateInput, NumberField, Show, SimpleShowLayout, ShowButton, DeleteButton } from 'react-admin';

import buildQuery from './buildQuery'; // see Specify your queries and mutations section below
import { ApolloConsumer } from '@apollo/client';

const UserList = (props) => (
    <List {...props}>
        <Datagrid>
            <TextField source="firstName" />
            <TextField source="lastName" />
            <TextField source="email" />
            <BooleanField source="isAdmin" sortable={false} />
            <DateField source="registrationDate" />
            <ShowButton />
            <EditButton />
            <DeleteButton />
        </Datagrid>
    </List>);

const UserEdit = (props) => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput source="firstName" />
            <TextInput source="lastName" />
            <BooleanInput source="isAdmin" />
            <NumberInput source="representsNumberParticipants" />
            <TextInput source="email" />
            <DateField source="registrationDate" />
        </SimpleForm>
    </Edit>);

const UserShow = (props) => (
    <Show {...props}>
        <SimpleShowLayout>
            <TextField source="firstName" />
            <TextField source="lastName" />
            <BooleanField source="isAdmin" />
            <NumberField source="representsNumberParticipants" />
            <TextField source="email" />
            <DateField source="registrationDate" />
        </SimpleShowLayout>
    </Show>);

const ProjectList = (props) => (
    <List {...props}>
        <Datagrid>
            <TextField source="name" />
            <TextField source="goal" />
            <TextField source="owner.fullName" label="Owner" sortable={false} />
            <DateField source="start" />
            <DateField source="end" />
            <ShowButton />
            <EditButton />
            <DeleteButton />
        </Datagrid>
    </List>);

const ProjectEdit = (props) => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput source="name" />
        </SimpleForm>
    </Edit>);

const ProjectShow = (props) => (
    <Show {...props}>
        <SimpleShowLayout>
            <TextField source="name" />
            <TextField source="goal" />
            <TextField source="owner.fullName" label="Owner" />
            <DateField source="start" />
            <DateField source="end" />
        </SimpleShowLayout>
    </Show>);

class AdminPageInner extends Component {
    constructor() {
        super();
        this.state = { dataProvider: null };
    }

    componentDidMount() {
        buildGraphQLProvider({ buildQuery, client: this.props.client })
            .then(dataProvider => this.setState({ dataProvider }));
    }

    render() {
        const { dataProvider } = this.state;

        if (!dataProvider) {
            return <div>Loading</div>;
        }

        return (
            <Admin dataProvider={dataProvider}>
                <Resource name="Project" list={ProjectList} edit={ProjectEdit} show={ProjectShow} />
                <Resource name="User" list={UserList} edit={UserEdit} show={UserShow} />
            </Admin>
        );
    }
}

const AdminPage = () => (
    <ApolloConsumer>
        {client => <AdminPageInner client={client} />}
    </ApolloConsumer>
);

export default AdminPage;