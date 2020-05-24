import React, { useState, useEffect } from "react";
import { gql, useQuery, useMutation, DataProxy, FetchResult } from "@apollo/client";
import Loader from "../Loader/Loader";
import { Card, CardContent, Grid, CardActions } from "@material-ui/core";
import Formatter from "../../formatter";
import { Button } from "../Button/Button";
import { useUser } from '../../providers/UserProvider';
import { Alert } from "../Alert/Alert";
import { Form, Formik } from "formik";
import * as Yup from "yup";
import { RichTextEditorFormControl } from "../RichTextEditorFormContol/RichTextEditorFormControl";

interface ICrowdactionCommentsProps {
    id: string;
}

const DELETE_COMMENT = gql`
  mutation($commentId: ID!) {
    crowdaction {
      deleteComment(commentId: $commentId)
    }
  }`;

const CREATE_COMMENT = gql`
  mutation($crowdactionId: ID!, $comment: String!) {
    crowdaction {
      createComment(crowdactionId: $crowdactionId, comment: $comment) {
        id
        comment
        commentedAt
        user {
          id
          fullName
        }
      }
    }
  }`;

const GET_COMMENTS = gql`
  query($crowdactionId: ID!, $take: Int!, $before: String!) {
    comments: crowdactionComments(crowdactionId: $crowdactionId, take: $take, where: [{ path: "id", comparison: lessThanOrEqual, value: [$before]}], orderBy: [{ path: "id", descending: true}]) {
      id
      comment
      commentedAt
      user {
        id
        fullName
      }
    }
  }`;

const numCommentsPerFetch = 20;
const maxCursor = "2147483647";

// Remove the comment from the cache
const updateCacheAfterDelete = (cache: DataProxy, mutationResult: FetchResult, id: string) => {
  if (mutationResult.data) {
    const removedId: number = mutationResult.data.crowdaction.deleteComment;

    const vars = {
      crowdactionId: id,
      take: numCommentsPerFetch,
      before: maxCursor
    };
    const comments: any = cache.readQuery({
      query: GET_COMMENTS,
      variables: vars
    });
    const toRemove = 
      comments.comments
              .findIndex((c: any) => c.id === removedId);

    if (toRemove >= 0) {
      const currentComments = comments.comments;
      const filteredComments = currentComments.slice(0, toRemove)
                                              .concat(currentComments.slice(toRemove + 1));
      cache.writeQuery({
        query: GET_COMMENTS,
        variables: vars,
        data: {
          comments: filteredComments
        }
      });
      return;
    };
  }
}

// Add the comment to the top of the comment list by putting it in the cache
const updateCacheAfterCreate = (cache: DataProxy, mutationResult: FetchResult, id: string) => {
  if (mutationResult.data) {
    const newComment = mutationResult.data.crowdaction.createComment;
    const vars = { crowdactionId: id, before: maxCursor, take: numCommentsPerFetch };
    const currentComments: any = cache.readQuery({
      query: GET_COMMENTS,
      variables: vars
    });
    cache.writeQuery({
      query: GET_COMMENTS,
      variables: vars,
      data: {
        comments: [ newComment, ...currentComments.comments ],
      }
    });
  }
}

// Put the new comments at the end of the list
const updateCacheAfterFetch = (previousResult: any, { fetchMoreResult }: any) => {
  return {
    comments: [...previousResult.comments, ...fetchMoreResult.comments],
  };
}

export default ({ id }: ICrowdactionCommentsProps) => {
  const user = useUser();
  const [ commentChangeNum, setCommentChangeNum ] = useState(0); // The net count difference in comments, counting deletions and new comments
  const { data, loading, fetchMore, error } = useQuery<any>(
    GET_COMMENTS,
    {
      variables: {
        crowdactionId: id,
        take: numCommentsPerFetch,
        before: maxCursor // Use the ID as cursor so we can have a 'stable' comment pagination
      }
    }
  );
  const comments = data?.comments;
  const lastId = 
    comments && comments.length > 0 ? (comments[comments.length - 1].id - 1).toString() :
                                      maxCursor;
  const [ deleteComment ] = useMutation(
    DELETE_COMMENT,
    {
      update: (cache: DataProxy, mutationResult: FetchResult) => {
        updateCacheAfterDelete(cache, mutationResult, id);
        setCommentChangeNum(commentChangeNum - 1);
      }
    });
  const [ createComment ] = useMutation(CREATE_COMMENT,
    {
      update: (cache: DataProxy, mutationResult: FetchResult) => {
        updateCacheAfterCreate(cache, mutationResult, id);
        setCommentChangeNum(commentChangeNum + 1);
      }
    });
  const loadMore = () => {
    fetchMore({
      variables: {
        before: lastId
      },
      updateQuery: (previousResult, options) => {
        return updateCacheAfterFetch(previousResult, options);
      }
    })        
  };

  useEffect(() => {
    if (error) {
      console.error(error?.message);
    }
  }, [ error ]);

  const validationSchema = Yup.object({
    comment: Yup.string()
  });
  
  const onSubmit = async (values: any, { setSubmitting, resetForm }: any) => { 
    await createComment({
      variables: { comment: values.comment, crowdactionId: id }
    });
    resetForm({});
    setSubmitting(false);
  };

  return <>
      <Grid container spacing={4}>
        <Alert type="error" text={error?.message} />
        {
          user ?
              <Grid item xs={12}>
                <Formik initialValues={{ comment: '' }} validationSchema={validationSchema} onSubmit={onSubmit}>
                  {(formik) =>
                    <Form>
                      <RichTextEditorFormControl
                        key={formik.submitCount} // Hack to ensure the component resets after submitting..
                        formik={formik}
                        height="130px"
                        name="comment"
                        label=""
                        hint="Start a conversation!"
                        fullWidth />
                      <Button type="submit">Comment</Button>
                    </Form>
                  }
                </Formik>
              </Grid>
              : null
        }
        {
          comments?.map((comment: any) => {
              const commentDate = new Date(comment.commentedAt);
              return <Grid key={comment.id} item xs={12}>
                <Card>
                  <CardContent>
                    <h4>{ comment.user.fullName }</h4>
                    <span dangerouslySetInnerHTML={{ __html: comment.comment}} />
                  </CardContent>
                  <CardActions>
                    <em>{ Formatter.date(commentDate) } { Formatter.time(commentDate) }</em>
                    {
                      user?.isAdmin ? <Button onClick={() => deleteComment({ variables: { commentId: comment.id }})}>Delete Comment</Button>
                                    : null
                    }
                  </CardActions>
                </Card>
              </Grid>;
            })
        }
        { loading ? <Grid item xs={12}><Loader /></Grid> : null }
        { lastId !== maxCursor && (comments.length - commentChangeNum) % numCommentsPerFetch === 0 ? 
            <Grid item xs={12}><Button onClick={() => loadMore()}>Load More</Button></Grid> : 
            null
        }
      </Grid>
  </>;
}