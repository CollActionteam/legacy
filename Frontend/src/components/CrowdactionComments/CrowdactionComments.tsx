import React, { useState, useEffect } from 'react';
import {
  gql,
  useQuery,
  useMutation,
  DataProxy,
  FetchResult,
} from '@apollo/client';
import Loader from '../Loader/Loader';
import { Grid, Box, TextField } from '@material-ui/core';
import Formatter from '../../formatter';
import { Button } from '../Button/Button';
import { useUser } from '../../providers/UserProvider';
import { Alert } from '../Alert/Alert';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { RichTextEditorFormControl } from '../RichTextEditorFormContol/RichTextEditorFormControl';
import styles from './CrowdactionComments.module.scss';
import { ICrowdactionComment } from '../../api/types';

interface ICrowdactionCommentsProps {
  id: string;
}

const DELETE_COMMENT = gql`
  mutation($commentId: ID!) {
    crowdaction {
      deleteComment(commentId: $commentId)
    }
  }
`;

const CREATE_COMMENT_ANONYMOUS = gql`
  mutation($crowdactionId: ID!, $comment: String!, $name: String!) {
    crowdaction {
      createCommentAnonymous(crowdactionId: $crowdactionId, comment: $comment, name: $name) {
        id
        comment
        commentedAt
        anonymousCommentUser
        user {
          id
          fullName
        }
      }
    }
  }
`;

const CREATE_COMMENT = gql`
  mutation($crowdactionId: ID!, $comment: String!) {
    crowdaction {
      createComment(crowdactionId: $crowdactionId, comment: $comment) {
        id
        comment
        commentedAt
        anonymousCommentUser
        user {
          id
          fullName
        }
      }
    }
  }
`;

const GET_COMMENTS = gql`
  query($crowdactionId: ID!, $take: Int!, $before: String!) {
    comments: crowdactionComments(
      crowdactionId: $crowdactionId,
      status: APPROVED,
      take: $take,
      where: [{ path: "id", comparison: lessThanOrEqual, value: [$before] }],
      orderBy: [{ path: "id", descending: true }]
    ) {
      id
      comment
      commentedAt
      anonymousCommentUser
      user {
        id
        fullName
      }
    }
  }
`;

const numCommentsPerFetch = 20;
const maxCursor = '2147483647';

// Remove the comment from the cache
const updateCacheAfterDelete = (
  cache: DataProxy,
  mutationResult: FetchResult,
  id: string
) => {
  if (mutationResult.data) {
    const removedId: number = mutationResult.data.crowdaction.deleteComment;

    const vars = {
      crowdactionId: id,
      take: numCommentsPerFetch,
      before: maxCursor,
    };
    const comments: any = cache.readQuery({
      query: GET_COMMENTS,
      variables: vars,
    });
    const toRemove = comments.comments.findIndex(
      (c: any) => c.id === removedId
    );

    if (toRemove >= 0) {
      const currentComments = comments.comments;
      const filteredComments = currentComments
        .slice(0, toRemove)
        .concat(currentComments.slice(toRemove + 1));
      cache.writeQuery({
        query: GET_COMMENTS,
        variables: vars,
        data: {
          comments: filteredComments,
        },
      });
      return;
    }
  }
};

// Add the comment to the top of the comment list by putting it in the cache
const updateCacheAfterCreate = (
  cache: DataProxy,
  mutationResult: FetchResult,
  id: string
) => {
  if (mutationResult.data) {
    const newComment = mutationResult.data.crowdaction.createComment;
    const vars = {
      crowdactionId: id,
      before: maxCursor,
      take: numCommentsPerFetch,
    };
    const currentComments: any = cache.readQuery({
      query: GET_COMMENTS,
      variables: vars,
    });
    cache.writeQuery({
      query: GET_COMMENTS,
      variables: vars,
      data: {
        comments: [newComment, ...currentComments.comments],
      },
    });
  }
};

// Put the new comments at the end of the list
const updateCacheAfterFetch = (
  previousResult: any,
  { fetchMoreResult }: any
) => {
  return {
    comments: [...previousResult.comments, ...fetchMoreResult.comments],
  };
};

export default ({ id }: ICrowdactionCommentsProps) => {
  const user = useUser();
  const [hasPostedAnonymously, setHasPostedAnonymously] = useState(false);
  const [mutationError, setMutationError] = useState('');
  const [commentChangeNum, setCommentChangeNum] = useState(0); // The net count difference in comments, counting deletions and new comments
  const { data, loading, fetchMore, error } = useQuery<any>(GET_COMMENTS, {
    variables: {
      crowdactionId: id,
      take: numCommentsPerFetch,
      before: maxCursor, // Use the ID as cursor so we can have a 'stable' comment pagination
    },
  });
  const comments: ICrowdactionComment[] | undefined = data?.comments;
  const lastId =
    comments && comments.length > 0
      ? (parseInt(comments[comments.length - 1].id) - 1).toString()
      : maxCursor;
  const [deleteComment] = useMutation(DELETE_COMMENT, {
    update: (cache: DataProxy, mutationResult: FetchResult) => {
      updateCacheAfterDelete(cache, mutationResult, id);
      setCommentChangeNum(commentChangeNum - 1);
    },
    onError: (errorData) => {
      console.error(errorData.message);
      setMutationError(errorData.message);
    },
  });
  const [createComment] = useMutation(CREATE_COMMENT, {
    update: (cache: DataProxy, mutationResult: FetchResult) => {
      updateCacheAfterCreate(cache, mutationResult, id);
      setCommentChangeNum(commentChangeNum + 1);
    },
    onError: (errorData) => {
      console.error(errorData.message);
      setMutationError(errorData.message);
    },
  });
  const [createCommentAnonymous] = useMutation(CREATE_COMMENT_ANONYMOUS, {
    onError: (errorData) => {
      console.error(errorData.message);
      setMutationError(errorData.message);
    },
  });
  const loadMore = () => {
    fetchMore({
      variables: {
        before: lastId,
      },
      updateQuery: (previousResult, options) => {
        return updateCacheAfterFetch(previousResult, options);
      },
    });
  };

  useEffect(() => {
    if (error) {
      console.error(error?.message);
    }
  }, [error]);

  const validationSchema = Yup.object({
    name: user ? Yup.string() : Yup.string().required(
      'You must add a name to post the comment'
    ).max(20, 'A name can be a maximum of 20 characters'),
    comment: Yup.string().required(
      'You must add a comment in the comment field'
    ),
  });

  const onSubmit = async (values: any, { setSubmitting, resetForm }: any) => {
    if (user) {
      await createComment({
        variables: { comment: values.comment, crowdactionId: id },
      });
    } else {
      await createCommentAnonymous({
        variables: { comment: values.comment, name: values.name, crowdactionId: id },
      });
      setHasPostedAnonymously(true);
    }
    resetForm({});
    setSubmitting(false);
  };

  // If the comments returned are an exact multiple of "numCommentsPerFetch"
  // then there are probably more. The only exception is if there are exactly
  // a multiple of "numCommentsPerFetch" in the database.
  // Bit of a performance trick, stops us from needing an extra count query.
  const probablyHasMoreComments =
    lastId !== maxCursor &&
    comments &&
    (comments.length - commentChangeNum) % numCommentsPerFetch === 0;

  return (
    <>
      <Grid container spacing={4}>
        <Alert type="error" text={error?.message} />
        <Alert type="error" text={mutationError} />
        <Grid item xs={12} className={styles.commentContainer}>
          <Formik
            initialValues={{ comment: '', name: '' }}
            validationSchema={validationSchema}
            onSubmit={onSubmit}
          >
            {(formik) => (
              <div className={styles.comment}>
                <Form className={styles.form}>
                  { !user && <Alert type="warning" text="Comments placed when not logged in need to be approved by a administrator before they're posted to crowdaction page. This process might take a few days. To publish your comment immediately, log in with your account." /> }
                  { hasPostedAnonymously && <Alert type="info" text="Your comment has been posted. It might take a few days for a administrator to approve your comment." /> }
                  { !user &&
                      <TextField
                          fullWidth
                          label="Name"
                          type="text"
                          { ...formik.getFieldProps('name') }
                      /> 
                  }
                  <RichTextEditorFormControl
                    key={formik.submitCount} // Hack to ensure the component resets after submitting..
                    formik={formik}
                    height="130px"
                    name="comment"
                    label=""
                    hint="Write a comment"
                    fullWidth
                  />
                  <div className={styles.commentButtonContainer}>
                    <Button type="submit">Comment</Button>
                  </div>
                </Form>
              </div>
            )}
          </Formik>
        </Grid>
        {comments?.map((comment: ICrowdactionComment) => {
          const commentDate = new Date(comment.commentedAt);
          return (
            <Grid
              key={comment.id}
              item
              xs={12}
              className={styles.commentContainer}
            >
              <div className={styles.comment}>
                <h4>{comment.user?.fullName ?? comment.anonymousCommentUser}</h4>
                <span dangerouslySetInnerHTML={{ __html: comment.comment }} />
                <span className={styles.commentDetails}>
                  {Formatter.date(commentDate)} {Formatter.time(commentDate)}
                </span>
                {user?.isAdmin ? (
                  <div className={styles.commentButtonContainer}>
                    <Button
                      onClick={() =>
                        deleteComment({ variables: { commentId: comment.id } })
                      }
                    >
                      Delete Comment
                    </Button>
                  </div>
                ) : null}
              </div>
            </Grid>
          );
        })}
        {loading ? (
          <Grid item xs={12}>
            <Loader />
          </Grid>
        ) : null}
        {probablyHasMoreComments ? (
          <Grid item xs={12}>
            <Box display="flex" justifyContent="center">
              <Button onClick={() => loadMore()}>Load More</Button>
            </Box>
          </Grid>
        ) : null}
      </Grid>
    </>
  );
};
