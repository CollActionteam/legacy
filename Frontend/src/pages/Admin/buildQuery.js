import { gql } from '@apollo/client';
import {
    GET_LIST,
    GET_ONE,
    GET_MANY,
    GET_MANY_REFERENCE,
    CREATE,
    UPDATE,
    UPDATE_MANY,
    DELETE,
    DELETE_MANY
} from 'ra-core';

const buildFieldList = (resource) => {
    switch (resource) {
        case 'User':
            return `
                id
                firstName
                lastName
                fullName
                email
                isAdmin
                representsNumberParticipants
                registrationDate`;
        case 'Project':
            return `
                id
                anonymousUserParticipants
                bannerImage {
                    id
                    url
                }
                descriptiveImage {
                    id
                    url
                }
                categories {
                    category
                }
                creatorComments
                description
                descriptionVideoLink
                displayPriority
                start
                end
                goal
                name
                numberProjectEmailsSend
                ownerId
                owner {
                    id
                    fullName
                }
                status
                tags {
                    tag {
                        name
                    }
                }
                proposal
                target
            `;
    }
}

const buildQuery = _introspectionResults => (raFetchType, resourceName, params) => {
    switch (raFetchType) {
        case GET_ONE:
            return {
                query: gql`query ${resourceName}($id: ID!) {
                    data: ${resourceName.toLowerCase()}(id: $id) {
                        ${buildFieldList(resourceName)}
                    }
                }`,
                variables: params,
                parseResponse: response => response.data,
            };
        case GET_MANY:
            return {
                query: gql`query ${resourceName}s($ids: [ID]) {
                    data: ${resourceName.toLowerCase()}s(ids: $ids) {
                        ${buildFieldList(resourceName)}
                    }
                }`,
                variables: params,
                parseResponse: response => response.data,
            };
        case GET_LIST:
            return {
                query: gql`query ${resourceName}s($take: Int, $skip: Int, $orderBy: [OrderByGraph], $where: [WhereExpressionGraph]) {
                    data: ${resourceName.toLowerCase()}s(take: $take, skip: $skip, orderBy: $orderBy, where: $where) {
                        ${buildFieldList(resourceName)}
                    }
                    total: ${resourceName.toLowerCase()}Count
                }`,
                variables: {
                    take: params.pagination?.perPage,
                    skip: params.pagination ? (params.pagination.page - 1) * params.pagination.perPage : null,
                    orderBy: params.sort ? [ { path: params.sort.field, descending: params.sort.order === "DESC" } ] : null,
                    where: params.filter ? Object.keys(params.filter).map((key) => { return { path: key, value: [ params.filter[key] ] }; }) : null
                },
                parseResponse: response => response.data,
            };
        case UPDATE:
            return {
                query: gql`mutation Update${resourceName}($entity: Updated${resourceName}InputGraph) {
                    data: ${resourceName.toLowerCase()} {
                        update: update${resourceName}(${resourceName.toLowerCase()}: $entity) {
                            succeeded
                            errors {
                                code description
                            }
                        }
                    }
                }`,
                variables: {
                    entity: params.data
                },
                parseResponse: response => {
                    if (response.data.update.succeeded) {
                        return params.data;
                    } else {
                        throw `Error updating: ${response.data.update.errors.map(e => e.description).join(", ")}`;
                    }
                },
            };
        case DELETE:
            return {
                query: gql`mutation Delete${resourceName}($id: ID!) {
                    data: ${resourceName.toLowerCase()} {
                        delete: delete${resourceName}(id: $id) {
                            id
                        }
                    }
                }`,
                variables: params,
                parseResponse: response => {
                    if (response.data.update.succeeded) {
                        return params.data;
                    } else {
                        throw `Error updating: ${response.data.update.errors.map(e => e.description).join(", ")}`;
                    }
                },
            };
        case DELETE_MANY:
            break;
        case UPDATE_MANY:
            throw "Update many not allowed"
        case GET_MANY_REFERENCE:
            throw "No referenced objects";
        case CREATE:
            throw "No creation of new objects possible";
    }
}

export default buildQuery;